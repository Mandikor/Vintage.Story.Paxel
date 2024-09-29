using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Paxel.Configuration;
using Vintagestory.GameContent;
using Vintagestory.API.Config;

namespace Paxel;

public class ItemPaxel : ItemAxe
{
    public SkillItem[] toolModes;

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);

        string PaxMat = Variant["material"];

        if (api.Side != EnumAppSide.Client) return;

        if (api is ICoreClientAPI capi)
        {

            if (PaxMat is "flint" or "bone-flint" or "obsidian" or "bone-obsidian" or "quartzite" or "bone-quartzite")
            {
                toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", delegate
                {
                    SkillItem[] modes = new SkillItem[]
                    {
                        new SkillItem
                        {
                            Code = new AssetLocation("1x1"),
                            Name = Lang.Get("paxel:1x1", Array.Empty<object>())
                        }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                        new SkillItem
                        {
                            Code = new AssetLocation("1x2"),
                            Name = Lang.Get("paxel:1x2", Array.Empty<object>())
                        }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2.svg"), 48, 48, 0, ColorUtil.WhiteArgb))
                    };

                    SkillItem[] array = modes;
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].TexturePremultipliedAlpha = false;
                    }

                    return modes;
                });
            }
            else
            {
                if (PaxMat is "silver" or "gold" or "electrum" or "copper")
                {
                    toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", delegate
                    {
                        SkillItem[] modes = new SkillItem[]
                        {
                            new SkillItem()
                            {
                                Code = new AssetLocation("1x1"),
                                Name = Lang.Get("paxel:1x1", Array.Empty<object>())
                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                            new SkillItem()
                            {
                                Code = new AssetLocation("1x2"),
                                Name = Lang.Get("paxel:1x2", Array.Empty<object>())
                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                            new SkillItem()
                            {
                                Code = new AssetLocation("1x3"),
                                Name = Lang.Get("paxel:1x3", Array.Empty<object>())
                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb))
                        };

                        SkillItem[] array = modes;
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].TexturePremultipliedAlpha = false;
                        }

                        return modes;
                    });
                }
                else
                {
                    if (PaxMat is "tinbronze" or "bismuthbronze" or "blackbronze")
                    {
                        toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", delegate
                        {
                            SkillItem[] modes = new SkillItem[]
                            {
                                new SkillItem()
                                {
                                    Code = new AssetLocation("1x1"),
                                    Name = Lang.Get("paxel:1x1", Array.Empty<object>())
                                }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                new SkillItem()
                                {
                                    Code = new AssetLocation("1x2"),
                                    Name = Lang.Get("paxel:1x2", Array.Empty < object >())
                                }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                new SkillItem()
                                {
                                    Code = new AssetLocation("1x3"),
                                    Name = Lang.Get("paxel:1x3", Array.Empty < object >())
                                }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                new SkillItem()
                                {
                                    Code = new AssetLocation("chess"),
                                    Name = Lang.Get("paxel:chessboard", Array.Empty < object >())
                                }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/chess.svg"), 48, 48, 0, ColorUtil.WhiteArgb))
                            };

                            SkillItem[] array = modes;
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i].TexturePremultipliedAlpha = false;
                            }

                            return modes;
                        });
                    }
                    else
                    {
                        if (PaxMat is "cupronickel" or "platinum")
                        {
                            toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", delegate
                            {
                                SkillItem[] modes = new SkillItem[]
                                {
                                    new SkillItem()
                                    {
                                        Code = new AssetLocation("1x1"),
                                        Name = Lang.Get("paxel:1x1", Array.Empty<object>())
                                    }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                    new SkillItem()
                                    {
                                        Code = new AssetLocation("1x2"),
                                        Name = Lang.Get("paxel:1x2", Array.Empty<object>())
                                    }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                    new SkillItem()
                                    {
                                        Code = new AssetLocation("1x3"),
                                        Name = Lang.Get("paxel:1x3", Array.Empty<object>())
                                    }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                    new SkillItem()
                                    {
                                        Code = new AssetLocation("3x3"),
                                        Name = Lang.Get("paxel:3x3", Array.Empty<object>())
                                    }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/3x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                    new SkillItem()
                                    {
                                        Code = new AssetLocation("chess"),
                                        Name = Lang.Get("paxel:chessboard", Array.Empty<object>())
                                    }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/chess.svg"), 48, 48, 0, ColorUtil.WhiteArgb))
                                };

                                SkillItem[] array = modes;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    array[i].TexturePremultipliedAlpha = false;
                                }

                                return modes;
                            });
                        }
                        else
                        {
                            if (PaxMat is "iron" or "meteoriciron")
                            {
                                toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", delegate
                                {
                                    SkillItem[] modes = new SkillItem[]
                                    {
                                        new SkillItem() 
                                        { 
                                            Code = new AssetLocation("1x1"), 
                                            Name = Lang.Get("paxel:1x1", Array.Empty<object>()) 
                                        }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                        new SkillItem() 
                                        { 
                                            Code = new AssetLocation("1x2"), 
                                            Name = Lang.Get("paxel:1x2", Array.Empty<object>()) 
                                        }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                        new SkillItem() 
                                        { 
                                            Code = new AssetLocation("1x3"), 
                                            Name = Lang.Get("paxel:1x3", Array.Empty<object>()) 
                                        }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                        new SkillItem() 
                                        { 
                                            Code = new AssetLocation("3x3"), 
                                            Name = Lang.Get("paxel:3x3", Array.Empty<object>()) 
                                        }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/3x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                        new SkillItem() 
                                        { 
                                            Code = new AssetLocation("5x5"), 
                                            Name = Lang.Get("paxel:5x5", Array.Empty<object>()) 
                                        }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/5x5.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                        new SkillItem() 
                                        { 
                                            Code = new AssetLocation("chess"), 
                                            Name = Lang.Get("paxel:chessboard", Array.Empty<object>()) 
                                        }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/chess.svg"), 48, 48, 0, ColorUtil.WhiteArgb))
                                    };

                                    SkillItem[] array = modes;
                                    for (int i = 0; i < array.Length; i++)
                                    {
                                        array[i].TexturePremultipliedAlpha = false;
                                    }

                                    return modes;
                                });
                            }
                            else
                            {
                                if (PaxMat is "steel" or "uranium" or "stainlesssteel" or "titanium")
                                {
                                    toolModes = ObjectCacheUtil.GetOrCreate(api, "paxelToolModes", delegate
                                    {
                                        SkillItem[] modes = new SkillItem[]
                                        {
                                            new SkillItem() 
                                            { 
                                                Code = new AssetLocation("1x1"), 
                                                Name = Lang.Get("paxel:1x1", Array.Empty<object>()) 
                                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x1.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                            new SkillItem() 
                                            { 
                                                Code = new AssetLocation("1x2"), 
                                                Name = Lang.Get("paxel:1x2", Array.Empty<object>()) 
                                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x2.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                            new SkillItem() 
                                            { 
                                                Code = new AssetLocation("1x3"), 
                                                Name = Lang.Get("paxel:1x3", Array.Empty<object>()) 
                                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/1x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                            new SkillItem() 
                                            { 
                                                Code = new AssetLocation("3x3"), 
                                                Name = Lang.Get("paxel:3x3", Array.Empty<object>()) 
                                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/3x3.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                            new SkillItem() 
                                            { 
                                                Code = new AssetLocation("5x5"), 
                                                Name = Lang.Get("paxel:5x5", Array.Empty<object>()) 
                                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/5x5.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                            new SkillItem() 
                                            { 
                                                Code = new AssetLocation("chess"), 
                                                Name = Lang.Get("paxel:chessboard", Array.Empty<object>()) 
                                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/chess.svg"), 48, 48, 0, ColorUtil.WhiteArgb)),
                                            new SkillItem() 
                                            { 
                                                Code = new AssetLocation("veinmine"), 
                                                Name = Lang.Get("paxel:veinmining", Array.Empty<object>()) 
                                            }.WithIcon(capi, capi.Gui.LoadSvgWithPadding(new AssetLocation("paxel:textures/icons/vein.svg"), 48, 48, 0, ColorUtil.WhiteArgb))
                                        };

                                        SkillItem[] array = modes;
                                        for (int i = 0; i < array.Length; i++)
                                        {
                                            array[i].TexturePremultipliedAlpha = false;
                                        }

                                        return modes;
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public override void OnUnloaded(ICoreAPI api)
    {
        for (int i = 0; toolModes != null && i < toolModes.Length; i++)
        {
            toolModes[i]?.Dispose();
        }
    }

    public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
    {
        if (blockSel == null) return null;

        return toolModes;
    }

    public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel)
    {
        return slot.Itemstack.Attributes.GetInt("toolMode", 0);
    }

    public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel, int toolMode)
    {
        slot.Itemstack.Attributes.SetInt("toolMode", toolMode);
    }

    public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot itemSlot)
    {
        return ArrayExtensions.Append(base.GetHeldInteractionHelp(itemSlot), new WorldInteraction
        {
            ActionLangCode = "paxel:heldhelp-settoolmode",
            HotKeyCode = "toolmodeselect",
            MouseButton = EnumMouseButton.None
        });
    }

}
