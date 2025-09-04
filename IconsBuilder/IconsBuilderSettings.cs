using System.Drawing;
using System.Numerics;
using ExileCore2;
using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using ExileCore2.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MinimapIcons.IconsBuilder;

[Submenu]
public class IconsBuilderSettings
{
    public RangeNode<int> RunEveryXTicks { get; set; } = new RangeNode<int>(10, 1, 20);

    [Menu("Debug information about entities")]
    public ToggleNode LogDebugInformation { get; set; } = new ToggleNode(true);

    public ToggleNode HidePlayers { get; set; } = new ToggleNode(false);
    public ToggleNode HideMinions { get; set; } = new ToggleNode(false);
    public ToggleNode DeliriumText { get; set; } = new ToggleNode(false);
    public ToggleNode HideBurriedMonsters { get; set; } = new ToggleNode(false);
    public ToggleNode UseReplacementsForGameIconsWhenOutOfRange { get; set; } = new ToggleNode(true);
    public ToggleNode UseReplacementsForItemIconsWhenOutOfRange { get; set; } = new ToggleNode(true);

    [Menu("Default size")]
    public float SizeDefaultIcon { get; set; } = new RangeNode<int>(16, 1, 50);

    [Menu("Size NPC icon")]
    public RangeNode<int> SizeNpcIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size monster icon")]
    public RangeNode<int> SizeEntityWhiteIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size magic monster icon")]
    public RangeNode<int> SizeEntityMagicIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size rare monster icon")]
    public RangeNode<int> SizeEntityRareIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size unique monster icon")]
    public RangeNode<int> SizeEntityUniqueIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size Proximity monster icon")]
    public RangeNode<int> SizeEntityProximityMonsterIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size breach chest icon")]
    public RangeNode<int> SizeBreachChestIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size Heist chest icon")]
    public RangeNode<int> SizeHeistChestIcon { get; set; } = new RangeNode<int>(30, 1, 50);

    public RangeNode<int> ExpeditionChestIconSize { get; set; } = new RangeNode<int>(30, 1, 50);
    public RangeNode<int> SanctumChestIconSize { get; set; } = new RangeNode<int>(30, 1, 50);
    public RangeNode<int> SanctumGoldIconSize { get; set; } = new RangeNode<int>(30, 1, 50);

    [Menu("Size chests icon")]
    public RangeNode<int> SizeChestIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Show small chests")]
    public ToggleNode ShowSmallChest { get; set; } = new ToggleNode(false);

    [Menu("Size small chests icon")]
    public RangeNode<int> SizeSmallChestIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size misc icon")]
    public RangeNode<int> SizeMiscIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [Menu("Size shrine icon")]
    public RangeNode<int> SizeShrineIcon { get; set; } = new RangeNode<int>(10, 1, 50);

    [JsonIgnore]
    public ButtonNode ResetIcons { get; set; } = new();

    public ContentNode<CustomIconSettings> CustomIcons { get; set; } = new ContentNode<CustomIconSettings> { ItemFactory = () => new CustomIconSettings(), };

    [Submenu]
    public MonsterIconSettings MonsterIcons { get; set; } = new();
}

[Submenu]
public class CustomIconSettings
{
    public TextNode MetadataRegex { get; set; } = new("^$");
    public ColorNode Tint { get; set; } = new(Color.White);
    public RangeNode<float> Size { get; set; } = new(5, 1, 60);
    [JsonConverter(typeof(StringEnumConverter))]
    public MapIconsIndex Icon;

    public CustomIconSettings()
    {
        IconNode = new PickerNode(this);
    }

    [Menu("Color")]
    [JsonIgnore]
    public PickerNode IconNode { get; set; }

    [Submenu(RenderMethod = nameof(Render))]
    public class PickerNode(CustomIconSettings customIconSettings)
    {
        private string _filter = "";
        private bool _shown = false;

        public void Render()
        {
            if (ImGuiHelpers.IconButton("##icon", Vector2.One * 15, customIconSettings.Icon, customIconSettings.Tint.Value.ToImguiVec4()))
            {
                _shown = true;
            }

            if (_shown)
            {
                if (ImGuiHelpers.IconPickerWindow(customIconSettings.MetadataRegex, ref customIconSettings.Icon, customIconSettings.Tint.Value.ToImguiVec4(), ref _filter))
                {
                    _shown = false;
                }
            }
        }
    }
}

[Submenu]
public class MonsterIconSettings
{
    public Color WhiteMonsterTint = Color.Red;
    public Color MagicMonsterTint = Color.Blue;
    public Color RareMonsterTint = Color.Yellow;
    public Color UniqueMonsterTint = Color.Orange;

    public int WhiteMonsterSize = 10;
    public int MagicMonsterSize = 13;
    public int RareMonsterSize = 16;
    public int UniqueMonsterSize = 21;

    public bool ShowUniqueNames = true;

    [JsonConverter(typeof(StringEnumConverter))]
    public MapIconsIndex WhiteMonsterIcon;
    [JsonConverter(typeof(StringEnumConverter))]
    public MapIconsIndex MagicMonsterIcon;
    [JsonConverter(typeof(StringEnumConverter))]
    public MapIconsIndex RareMonsterIcon;
    [JsonConverter(typeof(StringEnumConverter))]
    public MapIconsIndex UniqueMonsterIcon;

    public MonsterIconSettings()
    {
        IconNode = new PickerNode(this);
    }

    [Menu("Color")]
    [JsonIgnore]
    public PickerNode IconNode { get; set; }

    [Submenu(RenderMethod = nameof(Render))]
    public class PickerNode(MonsterIconSettings monsterIconSettings)
    {
        private string _filter = "";
        private bool _whiteShown = false;
        private bool _magicShown = false;
        private bool _rareShown = false;
        private bool _uniqueShown = false;

        public void Render()
        {
            // Add some spacing and a header
            ImGui.TextColored(new Vector4(1.0f, 0.8f, 0.2f, 1.0f), "Monster Icon Customization");
            ImGui.Separator();
            ImGui.Spacing();
            
            if (
                ImGui.BeginTable(
                    "Monster Icons",
                    4,
                    ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg
                )
            )
            {
                ImGui.TableSetupColumn("Monster Type", ImGuiTableColumnFlags.WidthFixed, 120);
                ImGui.TableSetupColumn("Size", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn("Color", ImGuiTableColumnFlags.WidthFixed, 80);
                ImGui.TableSetupColumn("Icon", ImGuiTableColumnFlags.WidthFixed, 60);
                ImGui.TableHeadersRow(); 
                // White/Normal Monsters Row
                ImGui.PushID($"IconWhite");
                ImGui.TableNextRow(ImGuiTableRowFlags.None);
                ImGui.TableNextColumn();
                ImGui.TextColored(new Vector4(0.9f, 0.9f, 0.9f, 1.0f), "Normal Monsters");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Customize appearance for white/normal monsters");
                
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(140);
                ImGui.SliderInt(
                    $"##whiteSize",
                    ref monsterIconSettings.WhiteMonsterSize,
                    1,
                    60
                );
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Icon size (1-60 pixels)");
                
                ImGui.TableNextColumn();
                Vector4 colorVector = new(monsterIconSettings.WhiteMonsterTint.R / 255.0f, monsterIconSettings.WhiteMonsterTint.G / 255.0f, monsterIconSettings.WhiteMonsterTint.B / 255.0f, monsterIconSettings.WhiteMonsterTint.A / 255.0f);
                if (ImGui.ColorEdit4($"##whiteColor", ref colorVector, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.NoInputs))
                    monsterIconSettings.WhiteMonsterTint = Color.FromArgb((int)(colorVector.W * 255), (int)(colorVector.X * 255), (int)(colorVector.Y * 255), (int)(colorVector.Z * 255));
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Icon color and transparency");
                
                ImGui.TableNextColumn();
                // Set background color for the icon button to transparent
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
                
                if (
                    ImGuiHelpers.IconButton(
                        "##iconWhite",
                        Vector2.One * monsterIconSettings.WhiteMonsterSize,
                        monsterIconSettings.WhiteMonsterIcon,
                        monsterIconSettings.WhiteMonsterTint.ToImguiVec4()
                    )
                )
                {
                    _whiteShown = true;
                }

                if (_whiteShown)
                {
                    if (
                        ImGuiHelpers.IconPickerWindow(
                            "##white",
                            ref monsterIconSettings.WhiteMonsterIcon,
                            monsterIconSettings.WhiteMonsterTint.ToImguiVec4(),
                            ref _filter
                        )
                    )
                    {
                        _whiteShown = false;
                    }
                }
                // reset button styles
                
                ImGui.PopID();
                
                // Magic Monsters Row
                ImGui.PushID($"IconMagic");
                ImGui.TableNextRow(ImGuiTableRowFlags.None);
                ImGui.TableNextColumn();
                ImGui.TextColored(new Vector4(0.4f, 0.4f, 1.0f, 1.0f), "Magic Monsters");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Customize appearance for blue/magic monsters");
                
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(140);
                ImGui.SliderInt(
                    $"##magicSize",
                    ref monsterIconSettings.MagicMonsterSize,
                    1,
                    60
                );
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Icon size (1-60 pixels)");
                
                ImGui.TableNextColumn();
                colorVector = new(monsterIconSettings.MagicMonsterTint.R / 255.0f, monsterIconSettings.MagicMonsterTint.G / 255.0f, monsterIconSettings.MagicMonsterTint.B / 255.0f, monsterIconSettings.MagicMonsterTint.A / 255.0f);
                if (ImGui.ColorEdit4($"##magicColor", ref colorVector, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.NoInputs))
                    monsterIconSettings.MagicMonsterTint = Color.FromArgb((int)(colorVector.W * 255), (int)(colorVector.X * 255), (int)(colorVector.Y * 255), (int)(colorVector.Z * 255));
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Icon color and transparency");
                
                ImGui.TableNextColumn();
                if (
                    ImGuiHelpers.IconButton(
                        "##iconMagic",
                        Vector2.One * monsterIconSettings.MagicMonsterSize,
                        monsterIconSettings.MagicMonsterIcon,
                        monsterIconSettings.MagicMonsterTint.ToImguiVec4()
                    )
                )
                {
                    _magicShown = true;
                }

                if (_magicShown)
                {
                    if (
                        ImGuiHelpers.IconPickerWindow(
                            "##magic",
                            ref monsterIconSettings.MagicMonsterIcon,
                            monsterIconSettings.MagicMonsterTint.ToImguiVec4(),
                            ref _filter
                        )
                    )
                    {
                        _magicShown = false;
                    }
                }
  
                ImGui.PopID();
                
                // Rare Monsters Row
                ImGui.PushID($"IconRare");
                ImGui.TableNextRow(ImGuiTableRowFlags.None);
                ImGui.TableNextColumn();
                ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), "Rare Monsters");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Customize appearance for yellow/rare monsters");
                
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(140);
                ImGui.SliderInt(
                    $"##rareSize",
                    ref monsterIconSettings.RareMonsterSize,
                    1,
                    60
                );
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Icon size (1-60 pixels)");

                ImGui.TableNextColumn();
                colorVector = new(monsterIconSettings.RareMonsterTint.R / 255.0f, monsterIconSettings.RareMonsterTint.G / 255.0f, monsterIconSettings.RareMonsterTint.B / 255.0f, monsterIconSettings.RareMonsterTint.A / 255.0f);
                if (ImGui.ColorEdit4($"##rareColor", ref colorVector, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.NoInputs))
                    monsterIconSettings.RareMonsterTint = Color.FromArgb((int)(colorVector.W * 255), (int)(colorVector.X * 255), (int)(colorVector.Y * 255), (int)(colorVector.Z * 255));
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Icon color and transparency");
                
                ImGui.TableNextColumn();
                if (
                    ImGuiHelpers.IconButton(
                        "##iconRare",
                        Vector2.One * monsterIconSettings.RareMonsterSize,
                        monsterIconSettings.RareMonsterIcon,
                        monsterIconSettings.RareMonsterTint.ToImguiVec4()
                    )
                )
                {
                    _rareShown = true;
                }

                if (_rareShown)
                {
                    if (
                        ImGuiHelpers.IconPickerWindow(
                            "##rare",
                            ref monsterIconSettings.RareMonsterIcon,
                            monsterIconSettings.RareMonsterTint.ToImguiVec4(),
                            ref _filter
                        )
                    )
                    {
                        _rareShown = false;
                    }
                }

                ImGui.PopID();
                
                // Unique Monsters Row
                ImGui.PushID($"IconUnique");
                ImGui.TableNextRow(ImGuiTableRowFlags.None);
                ImGui.TableNextColumn();
                ImGui.TextColored(new Vector4(1.0f, 0.6f, 0.0f, 1.0f), "Unique Monsters");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Customize appearance for orange/unique monsters");
                
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(140);
                ImGui.SliderInt(
                    $"##uniqueSize",
                    ref monsterIconSettings.UniqueMonsterSize,
                    1,
                    60
                );
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Icon size (1-60 pixels)");
                
                ImGui.TableNextColumn();
                colorVector = new(monsterIconSettings.UniqueMonsterTint.R / 255.0f, monsterIconSettings.UniqueMonsterTint.G / 255.0f, monsterIconSettings.UniqueMonsterTint.B / 255.0f, monsterIconSettings.UniqueMonsterTint.A / 255.0f);
                if (ImGui.ColorEdit4($"##uniqueColor", ref colorVector, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.NoInputs))
                    monsterIconSettings.UniqueMonsterTint = Color.FromArgb((int)(colorVector.W * 255), (int)(colorVector.X * 255), (int)(colorVector.Y * 255), (int)(colorVector.Z * 255));
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Icon color and transparency");
                
                ImGui.TableNextColumn();
                if (
                    ImGuiHelpers.IconButton(
                        "##iconUnique",
                        Vector2.One * monsterIconSettings.UniqueMonsterSize,
                        monsterIconSettings.UniqueMonsterIcon,
                        monsterIconSettings.UniqueMonsterTint.ToImguiVec4()
                    )
                )
                {
                    _uniqueShown = true;
                }

                if (_uniqueShown)
                {
                    if (
                        ImGuiHelpers.IconPickerWindow(
                            "##unique",
                            ref monsterIconSettings.UniqueMonsterIcon,
                            monsterIconSettings.UniqueMonsterTint.ToImguiVec4(),
                            ref _filter
                        )
                    )
                    {
                        _uniqueShown = false;
                    }
                }
                
                ImGui.PopStyleColor();
                ImGui.PopID();
                ImGui.EndTable();
            }
            
            // Add spacing and unique names toggle outside the table
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
            
            ImGui.TextColored(new Vector4(1.0f, 0.6f, 0.0f, 1.0f), "Unique Monster Names");
            ImGui.Checkbox("Show Unique Monster Names", ref monsterIconSettings.ShowUniqueNames);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("Toggle visibility of unique monster names on the minimap");
        }
    }
}