using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using GameOffsets2.Native;

namespace MinimapIcons.IconsBuilder.Icons;

public class MonsterIcon : BaseIcon
{
    public MonsterIcon(Entity entity, IconsBuilderSettings settings, Dictionary<string, Vector2i> modIcons)
        : base(entity)
    {
        Update(entity, settings, modIcons);
    }

    public void Update(Entity entity, IconsBuilderSettings settings, Dictionary<string, Vector2i> modIcons)
    {
        Show = () => entity.IsAlive;
        if(entity.IsHidden && settings.HideBurriedMonsters)
        {
            Show = () => !entity.IsHidden && entity.IsAlive;
        }

        if (!_HasIngameIcon) MainTexture = new HudTexture("Icons.png");

        MainTexture.Size = Rarity switch
        {
            MonsterRarity.White => settings.MonsterIcons.WhiteMonsterSize,
            MonsterRarity.Magic => settings.MonsterIcons.MagicMonsterSize,
            MonsterRarity.Rare => settings.MonsterIcons.RareMonsterSize,
            MonsterRarity.Unique => settings.MonsterIcons.UniqueMonsterSize,
            _ => throw new ArgumentException($"{nameof(MonsterIcon)} wrong rarity for {entity.Path}. Dump: {entity.GetComponent<ObjectMagicProperties>().DumpObject()}")
        };

        if (_HasIngameIcon && entity.HasComponent<MinimapIcon>() && !entity.GetComponent<MinimapIcon>().Name.Equals("NPC"))
            return;

        if (!entity.IsHostile)
        {
            if (!_HasIngameIcon)
            {
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterSmallGreenCircle);
                Priority = IconPriority.Low;
                Show = () => !settings.HideMinions && entity.IsAlive;
            }

            //Spirits icon
        }
        else if (Rarity == MonsterRarity.Unique && entity.Path.Contains("Metadata/Monsters/Spirit/"))
            MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeGreenHexagon);
        else
        {
            string modName = null;

            if (entity.HasComponent<ObjectMagicProperties>())
            {
                var objectMagicProperties = entity.GetComponent<ObjectMagicProperties>();

                var mods = objectMagicProperties.Mods;

                if (mods != null)
                {
                    if (mods.Contains("MonsterConvertsOnDeath_")) Show = () => entity.IsAlive && entity.IsHostile;

                    modName = mods.FirstOrDefault(modIcons.ContainsKey);
                }
            }

            if (modName != null)
            {
                MainTexture = new HudTexture("sprites.png");
                MainTexture.UV = SpriteHelper.GetUV(modIcons[modName], new Vector2i(7, 8));
                Priority = IconPriority.VeryHigh;
            }
            else
            {
                switch (Rarity)
                {
                    case MonsterRarity.White:
                        MainTexture.UV = SpriteHelper.GetUV(settings.MonsterIcons.WhiteMonsterIcon);
                        MainTexture.Color = Color.FromArgb(
                            settings.MonsterIcons.WhiteMonsterTint.A,
                            settings.MonsterIcons.WhiteMonsterTint.R,
                            settings.MonsterIcons.WhiteMonsterTint.G,
                            settings.MonsterIcons.WhiteMonsterTint.B);
                        break;
                    case MonsterRarity.Magic:
                        MainTexture.UV = SpriteHelper.GetUV(settings.MonsterIcons.MagicMonsterIcon);
                        MainTexture.Color = Color.FromArgb(
                            settings.MonsterIcons.MagicMonsterTint.A,
                            settings.MonsterIcons.MagicMonsterTint.R,
                            settings.MonsterIcons.MagicMonsterTint.G,
                            settings.MonsterIcons.MagicMonsterTint.B);
                        break;
                    case MonsterRarity.Rare:
                        MainTexture.UV = SpriteHelper.GetUV(settings.MonsterIcons.RareMonsterIcon);
                        MainTexture.Color = Color.FromArgb(
                            settings.MonsterIcons.RareMonsterTint.A,
                            settings.MonsterIcons.RareMonsterTint.R,
                            settings.MonsterIcons.RareMonsterTint.G,
                            settings.MonsterIcons.RareMonsterTint.B);
                        break;
                    case MonsterRarity.Unique:
                        MainTexture.UV = SpriteHelper.GetUV(settings.MonsterIcons.UniqueMonsterIcon);
                        MainTexture.Color = Color.FromArgb(
                            settings.MonsterIcons.UniqueMonsterTint.A,
                            settings.MonsterIcons.UniqueMonsterTint.R,
                            settings.MonsterIcons.UniqueMonsterTint.G,
                            settings.MonsterIcons.UniqueMonsterTint.B);
                        if (settings.MonsterIcons.ShowUniqueNames)
                            Text = RenderName.Split(',').FirstOrDefault();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            $"Rarity wrong was is {Rarity}. {entity.GetComponent<ObjectMagicProperties>().DumpObject()}");
                }
            }
        }
    }
}