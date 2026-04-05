using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace TargetedMobBuff
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Name => "MobBuffController";
        public override string Author => "淦";
        public override string Description => "在指定怪物周围控制玩家Buff，可自定义持续时间";
        public override Version Version => new(2025, 4, 18, 4); // 版本号更新

        public Plugin(Main game) : base(game) { }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("mobbuff.use", MobBuffCmd, "mobbuff")
            {
                HelpText = "在指定怪物周围控制玩家Buff\n" +
                           "/mobbuff add <怪物ID/名称> <buffID> <范围> <持续时间(秒)> - 添加Buff\n" +
                           "/mobbuff del <怪物ID/名称> <buffID> <范围> - 移除Buff\n" +
                           "注意: 持续时间仅对add命令有效"
            });
        }

        private void MobBuffCmd(CommandArgs args)
        {
            string action = args.Parameters.Count > 0 ? args.Parameters[0].ToLower() : "";
            
            if (action != "add" && action != "del")
            {
                args.Player.SendErrorMessage("无效操作! 使用 'add' 或 'del'");
                args.Player.SendErrorMessage("示例: /mobbuff add 僵尸 1 20 30");
                return;
            }

            // 参数数量验证
            if ((action == "add" && args.Parameters.Count < 5) || 
                (action == "del" && args.Parameters.Count < 4))
            {
                args.Player.SendErrorMessage($"用法: /mobbuff {action} <怪物ID/名称> <buffID> <范围>{(action == "add" ? " <持续时间(秒)>" : "")}");
                return;
            }

            // 解析怪物标识
            string mobIdentifier = args.Parameters[1];
            List<int> mobIds = ResolveMobIdentifier(mobIdentifier);

            if (mobIds == null || mobIds.Count == 0)
            {
                args.Player.SendErrorMessage($"未找到匹配的怪物: {mobIdentifier}");
                return;
            }

            // 解析Buff ID
            if (!int.TryParse(args.Parameters[2], out int buffId) || buffId <= 0 || buffId >= BuffID.Count)
            {
                args.Player.SendErrorMessage("无效Buff ID! 使用 /bufflist 查看有效ID");
                return;
            }

            // 解析范围
            if (!float.TryParse(args.Parameters[3], out float range) || range <= 0)
            {
                args.Player.SendErrorMessage("范围必须是大于0的数字!");
                return;
            }

            // 解析持续时间（仅add命令需要）
            int duration = 36000; // 默认10分钟（600秒）
            if (action == "add")
            {
                if (!int.TryParse(args.Parameters[4], out int seconds) || seconds <= 0)
                {
                    args.Player.SendErrorMessage("持续时间必须是大于0的整数（秒）!");
                    return;
                }
                duration = seconds * 60; // 转换为帧数
            }

            int playersAffected = 0;
            float rangeSquared = range * range;

            // 遍历所有指定类型的NPC
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && mobIds.Contains(npc.type))
                {
                    // 遍历所有在线玩家
                    foreach (TSPlayer player in TShock.Players)
                    {
                        if (player != null && player.Active && 
                            player.TPlayer.DistanceSQ(npc.Center) < rangeSquared)
                        {
                            if (action == "add")
                            {
                                player.SetBuff(buffId, duration);
                                playersAffected++;
                            }
                            else
                            {
                                // 修复：使用兼容的方式移除buff
                                RemovePlayerBuff(player, buffId);
                                playersAffected++;
                            }
                        }
                    }
                }
            }

            string actionMsg = action == "add" ? $"添加 {duration/60}秒" : "移除";
            args.Player.SendSuccessMessage($"[{actionMsg} buff {buffId}] 在 {mobIdentifier} 周围影响 {playersAffected} 名玩家");
        }

        // 修复：兼容的buff移除方法
        private void RemovePlayerBuff(TSPlayer player, int buffId)
        {
            // 方法1：如果TSPlayer有RemoveBuff方法
            var removeMethod = player.GetType().GetMethod("RemoveBuff");
            if (removeMethod != null)
            {
                removeMethod.Invoke(player, new object[] { buffId });
                return;
            }

            // 方法2：通过SetBuff设置0持续时间
            player.SetBuff(buffId, 0);
            
            // 方法3：直接操作玩家buff数组（最兼容）
            for (int i = 0; i < player.TPlayer.buffType.Length; i++)
            {
                if (player.TPlayer.buffType[i] == buffId)
                {
                    player.TPlayer.buffType[i] = 0;
                    player.TPlayer.buffTime[i] = 0;
                }
            }
        }

        // 解析怪物标识符（支持ID或名称）
        private List<int> ResolveMobIdentifier(string identifier)
        {
            // 尝试按ID解析
            if (int.TryParse(identifier, out int id) && id > 0 && id < NPCID.Count)
            {
                return new List<int> { id };
            }

            // 按名称搜索（不区分大小写）
            identifier = identifier.ToLowerInvariant();
            List<int> matchedIds = new List<int>();
            
            for (int i = 1; i < NPCID.Count; i++)
            {
                string npcName = Lang.GetNPCNameValue(i).ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(npcName) && npcName.Contains(identifier))
                {
                    matchedIds.Add(i);
                }
            }
            
            return matchedIds;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}