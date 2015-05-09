﻿using System.Drawing;
using System.Linq;
using SpellWorkLib.DBC;
using SpellWorkLib.Extensions;

namespace SpellWorkLib.Spell
{
    public class SpellInfo
    {
        private readonly ISpellInfoWriter _spellInfoWriter;
        private SpellEntry _spell;

        public SpellInfo(ISpellInfoWriter spellInfoWriter, SpellEntry spell)
        {
            _spellInfoWriter = spellInfoWriter;
            _spell = spell;

            ViewSpellInfo();
        }

        public override string ToString()
        {
            return _spellInfoWriter.ToString();
        }

        private void ViewSpellInfo()
        {
            _spellInfoWriter.Clear();
            _spellInfoWriter.SetBold();
            _spellInfoWriter.AppendFormatLine("ID - {0} {1}", _spell.ID, _spell.SpellNameRank);
            _spellInfoWriter.SetDefaultStyle();

            _spellInfoWriter.AppendSeparator();
            _spellInfoWriter.AppendFormatLineIfNotNull("Description: {0}", _spell.Description);
            _spellInfoWriter.AppendFormatLineIfNotNull("ToolTip: {0}", _spell.ToolTip);
            _spellInfoWriter.AppendFormatLineIfNotNull("Modal Next Spell: {0}", _spell.ModalNextSpell);
            if (_spell.Description != string.Empty && _spell.ToolTip != string.Empty && _spell.ModalNextSpell != 0)
                _spellInfoWriter.AppendSeparator();

            _spellInfoWriter.AppendFormatLine("Category = {0}, SpellIconID = {1}, activeIconID = {2}, SpellVisual = ({3},{4})",
                _spell.Category, _spell.SpellIconID, _spell.ActiveIconID, _spell.SpellVisual[0], _spell.SpellVisual[1]);

            _spellInfoWriter.AppendFormatLine("Family {0}, flag [0] 0x{1:X8} [1] 0x{2:X8} [2] 0x{3:X8}",
                (SpellFamilyNames)_spell.SpellFamilyName, _spell.SpellFamilyFlags[0], _spell.SpellFamilyFlags[1], _spell.SpellFamilyFlags[2]);

            _spellInfoWriter.AppendLine();

            _spellInfoWriter.AppendFormatLine("SpellSchoolMask = {0} ({1})", _spell.SchoolMask, _spell.School);
            _spellInfoWriter.AppendFormatLine("DamageClass = {0} ({1})", _spell.DmgClass, (SpellDmgClass)_spell.DmgClass);
            _spellInfoWriter.AppendFormatLine("PreventionType = {0} ({1})", _spell.PreventionType, (SpellPreventionType)_spell.PreventionType);

            if (_spell.Attributes != 0 || _spell.AttributesEx != 0 || _spell.AttributesEx2 != 0 || _spell.AttributesEx3 != 0
                || _spell.AttributesEx4 != 0 || _spell.AttributesEx5 != 0 || _spell.AttributesEx6 != 0 || _spell.AttributesEx7 != 0)
                _spellInfoWriter.AppendSeparator();

            if (_spell.Attributes != 0)
                _spellInfoWriter.AppendFormatLine("Attributes: 0x{0:X8} ({1})", _spell.Attributes, (SpellAtribute)_spell.Attributes);
            if (_spell.AttributesEx != 0)
                _spellInfoWriter.AppendFormatLine("AttributesEx1: 0x{0:X8} ({1})", _spell.AttributesEx, (SpellAtributeEx)_spell.AttributesEx);
            if (_spell.AttributesEx2 != 0)
                _spellInfoWriter.AppendFormatLine("AttributesEx2: 0x{0:X8} ({1})", _spell.AttributesEx2, (SpellAtributeEx2)_spell.AttributesEx2);
            if (_spell.AttributesEx3 != 0)
                _spellInfoWriter.AppendFormatLine("AttributesEx3: 0x{0:X8} ({1})", _spell.AttributesEx3, (SpellAtributeEx3)_spell.AttributesEx3);
            if (_spell.AttributesEx4 != 0)
                _spellInfoWriter.AppendFormatLine("AttributesEx4: 0x{0:X8} ({1})", _spell.AttributesEx4, (SpellAtributeEx4)_spell.AttributesEx4);
            if (_spell.AttributesEx5 != 0)
                _spellInfoWriter.AppendFormatLine("AttributesEx5: 0x{0:X8} ({1})", _spell.AttributesEx5, (SpellAtributeEx5)_spell.AttributesEx5);
            if (_spell.AttributesEx6 != 0)
                _spellInfoWriter.AppendFormatLine("AttributesEx6: 0x{0:X8} ({1})", _spell.AttributesEx6, (SpellAtributeEx6)_spell.AttributesEx6);
            if (_spell.AttributesEx7 != 0)
                _spellInfoWriter.AppendFormatLine("AttributesEx7: 0x{0:X8} ({1})", _spell.AttributesEx7, (SpellAtributeEx7)_spell.AttributesEx7);

            _spellInfoWriter.AppendSeparator();

            if (_spell.Targets != 0)
                _spellInfoWriter.AppendFormatLine("Targets Mask = 0x{0:X8} ({1})", _spell.Targets, (SpellCastTargetFlags)_spell.Targets);

            if (_spell.TargetCreatureType != 0)
                _spellInfoWriter.AppendFormatLine("Creature Type Mask = 0x{0:X8} ({1})",
                    _spell.TargetCreatureType, (CreatureTypeMask)_spell.TargetCreatureType);

            if (_spell.Stances != 0)
                _spellInfoWriter.AppendFormatLine("Stances: {0}", (ShapeshiftFormMask)_spell.Stances);

            if (_spell.StancesNot != 0)
                _spellInfoWriter.AppendFormatLine("Stances Not: {0}", (ShapeshiftFormMask)_spell.StancesNot);

            AppendSkillLine();

            // reagents
            {
                var printedHeader = false;
                for (var i = 0; i < _spell.Reagent.Length; ++i)
                {
                    if (_spell.Reagent[i] == 0)
                        continue;

                    if (!printedHeader)
                    {
                        _spellInfoWriter.AppendLine();
                        _spellInfoWriter.Append("Reagents:");
                        printedHeader = true;
                    }

                    _spellInfoWriter.AppendFormat("  {0} x{1}", _spell.Reagent[i], _spell.ReagentCount[i]);
                }

                if (printedHeader)
                    _spellInfoWriter.AppendLine();
            }

            _spellInfoWriter.AppendFormatLine("Spell Level = {0}, base {1}, max {2}, maxTarget {3}",
                _spell.SpellLevel, _spell.BaseLevel, _spell.MaxLevel, _spell.MaxTargetLevel);

            if (_spell.EquippedItemClass != -1)
            {
                _spellInfoWriter.AppendFormatLine("EquippedItemClass = {0} ({1})", _spell.EquippedItemClass, (ItemClass)_spell.EquippedItemClass);

                if (_spell.EquippedItemSubClassMask != 0)
                {
                    switch ((ItemClass)_spell.EquippedItemClass)
                    {
                        case ItemClass.WEAPON:
                            _spellInfoWriter.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassWeaponMask)_spell.EquippedItemSubClassMask);
                            break;
                        case ItemClass.ARMOR:
                            _spellInfoWriter.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassArmorMask)_spell.EquippedItemSubClassMask);
                            break;
                        case ItemClass.MISC:
                            _spellInfoWriter.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassMiscMask)_spell.EquippedItemSubClassMask);
                            break;
                    }
                }

                if (_spell.EquippedItemInventoryTypeMask != 0)
                    _spellInfoWriter.AppendFormatLine("    InventoryType mask = 0x{0:X8} ({1})",
                        _spell.EquippedItemInventoryTypeMask, (InventoryTypeMask)_spell.EquippedItemInventoryTypeMask);
            }

            _spellInfoWriter.AppendLine();
            _spellInfoWriter.AppendFormatLine("Category = {0}", _spell.Category);
            _spellInfoWriter.AppendFormatLine("DispelType = {0} ({1})", _spell.Dispel, (DispelType)_spell.Dispel);
            _spellInfoWriter.AppendFormatLine("Mechanic = {0} ({1})", _spell.Mechanic, (Mechanics)_spell.Mechanic);

            _spellInfoWriter.AppendLine(_spell.Range);

            _spellInfoWriter.AppendFormatLineIfNotNull("Speed {0:F}", _spell.Speed);
            _spellInfoWriter.AppendFormatLineIfNotNull("Stackable up to {0}", _spell.StackAmount);

            _spellInfoWriter.AppendLine(_spell.CastTime);

            if (_spell.RecoveryTime != 0 || _spell.CategoryRecoveryTime != 0 || _spell.StartRecoveryCategory != 0)
            {
                _spellInfoWriter.AppendFormatLine("RecoveryTime: {0} ms, CategoryRecoveryTime: {1} ms", _spell.RecoveryTime, _spell.CategoryRecoveryTime);
                _spellInfoWriter.AppendFormatLine("StartRecoveryCategory = {0}, StartRecoveryTime = {1:F} ms", _spell.StartRecoveryCategory, _spell.StartRecoveryTime);
            }

            _spellInfoWriter.AppendLine(_spell.Duration);

            if (_spell.ManaCost != 0 || _spell.ManaCostPercentage != 0 || _spell.PowerType != 0 ||
                _spell.ManaCostPerlevel != 0 || _spell.ManaPerSecond != 0 || _spell.ManaPerSecondPerLevel != 0)
            {
                _spellInfoWriter.AppendFormat("Power {0}, Cost {1}",
                    (Powers)_spell.PowerType, _spell.ManaCost == 0 ? _spell.ManaCostPercentage + " %" : _spell.ManaCost.ToString());
                _spellInfoWriter.AppendFormatIfNotNull(" + lvl * {0}", _spell.ManaCostPerlevel);
                _spellInfoWriter.AppendFormatIfNotNull(" + {0} Per Second", _spell.ManaPerSecond);
                _spellInfoWriter.AppendFormatIfNotNull(" + lvl * {0}", _spell.ManaPerSecondPerLevel);
                _spellInfoWriter.AppendLine();
            }

            _spellInfoWriter.AppendFormatLine("Interrupt Flags: 0x{0:X8}, AuraIF 0x{1:X8}, ChannelIF 0x{2:X8}",
                _spell.InterruptFlags, _spell.AuraInterruptFlags, _spell.ChannelInterruptFlags);

            if (_spell.CasterAuraState != 0)
                _spellInfoWriter.AppendFormatLine("CasterAuraState = {0} ({1})", _spell.CasterAuraState, (AuraState)_spell.CasterAuraState);

            if (_spell.TargetAuraState != 0)
                _spellInfoWriter.AppendFormatLine("TargetAuraState = {0} ({1})", _spell.TargetAuraState, (AuraState)_spell.TargetAuraState);

            if (_spell.CasterAuraStateNot != 0)
                _spellInfoWriter.AppendFormatLine("CasterAuraStateNot = {0} ({1})", _spell.CasterAuraStateNot, (AuraState)_spell.CasterAuraStateNot);

            if (_spell.TargetAuraStateNot != 0)
                _spellInfoWriter.AppendFormatLine("TargetAuraStateNot = {0} ({1})", _spell.TargetAuraStateNot, (AuraState)_spell.TargetAuraStateNot);

            if (_spell.MaxAffectedTargets != 0)
                _spellInfoWriter.AppendFormatLine("MaxAffectedTargets = {0}", _spell.MaxAffectedTargets);

            AppendSpellAura();

            AppendAreaInfo();

            _spellInfoWriter.AppendFormatLineIfNotNull("Requires Spell Focus {0}", _spell.RequiresSpellFocus);

            if (_spell.ProcFlags != 0)
            {
                _spellInfoWriter.SetBold();
                _spellInfoWriter.AppendFormatLine("Proc flag 0x{0:X8}, chance = {1}, charges - {2}",
                _spell.ProcFlags, _spell.ProcChance, _spell.ProcCharges);
                _spellInfoWriter.SetDefaultStyle();
                _spellInfoWriter.AppendSeparator();
                _spellInfoWriter.AppendText(_spell.ProcInfo);
            }
            else
            {
                _spellInfoWriter.AppendFormatLine("Chance = {0}, charges - {1}", _spell.ProcChance, _spell.ProcCharges);
            }

            AppendSpellEffectInfo();
            AppendDifficultyInfo();

            AppendSpellVisualInfo();
        }

        private void AppendSpellVisualInfo()
        {
            SpellVisualEntry visualData;
            if (!DBC.DBC.SpellVisual.TryGetValue(_spell.SpellVisual[0], out visualData))
                return;

            SpellMissileEntry missileEntry;
            SpellMissileMotionEntry missileMotionEntry;
            var hasMissileEntry = DBC.DBC.SpellMissile.TryGetValue(visualData.MissileModel, out missileEntry);
            var hasMissileMotion = DBC.DBC.SpellMissileMotion.TryGetValue(visualData.MissileMotionId, out missileMotionEntry);

            if (!hasMissileEntry && !hasMissileMotion)
                return;

            _spellInfoWriter.AppendSeparator();
            _spellInfoWriter.SetBold();
            _spellInfoWriter.AppendLine("Missile data");
            _spellInfoWriter.SetDefaultStyle();

            // Missile Model Data.
            if (hasMissileEntry)
            {
                _spellInfoWriter.AppendFormatLine("Missile Model ID: {0}", visualData.MissileModel);
                _spellInfoWriter.AppendFormatLine("Missile attachment: {0}", visualData.MissileAttachment);
                _spellInfoWriter.AppendFormatLine("Missile cast offset: X:{0} Y:{1} Z:{2}", visualData.MissileCastOffsetX, visualData.MissileCastOffsetY, visualData.MissileCastOffsetZ);
                _spellInfoWriter.AppendFormatLine("Missile impact offset: X:{0} Y:{1} Z:{2}", visualData.MissileImpactOffsetX, visualData.MissileImpactOffsetY, visualData.MissileImpactOffsetZ);
                _spellInfoWriter.AppendFormatLine("MissileEntry ID: {0}", missileEntry.Id);
                _spellInfoWriter.AppendFormatLine("Collision Radius: {0}", missileEntry.collisionRadius);
                _spellInfoWriter.AppendFormatLine("Default Pitch: {0} - {1}", missileEntry.defaultPitchMin, missileEntry.defaultPitchMax);
                _spellInfoWriter.AppendFormatLine("Random Pitch: {0} - {1}", missileEntry.randomizePitchMax, missileEntry.randomizePitchMax);
                _spellInfoWriter.AppendFormatLine("Default Speed: {0} - {1}", missileEntry.defaultSpeedMin, missileEntry.defaultSpeedMax);
                _spellInfoWriter.AppendFormatLine("Randomize Speed: {0} - {1}", missileEntry.randomizeSpeedMin, missileEntry.randomizeSpeedMax);
                _spellInfoWriter.AppendFormatLine("Gravity: {0}", missileEntry.gravity);
                _spellInfoWriter.AppendFormatLine("Maximum duration:", missileEntry.maxDuration);
                _spellInfoWriter.AppendLine("");
            }

            // Missile Motion Data.
            if (hasMissileMotion)
            {
                _spellInfoWriter.AppendFormatLine("Missile motion: {0}", missileMotionEntry.Name);
                _spellInfoWriter.AppendFormatLine("Missile count: {0}", missileMotionEntry.MissileCount);
                _spellInfoWriter.AppendLine("Missile Script body:");
                _spellInfoWriter.AppendText(missileMotionEntry.Script);
            }
        }

        private void AppendSkillLine()
        {
            var query = (from skillLineAbility in DBC.DBC.SkillLineAbility
                        join skillLine in DBC.DBC.SkillLine
                        on skillLineAbility.Value.SkillId equals skillLine.Key
                        where skillLineAbility.Value.SpellId == _spell.ID
                        select new
                        {
                            skillLineAbility,
                            skillLine
                        }).FirstOrDefault();

            if (query == null)
                return;

            var skill = query.skillLineAbility.Value;
            var line = query.skillLine.Value;

            _spellInfoWriter.AppendFormatLine("Skill (Id {0}) \"{1}\"", skill.SkillId, line.Name);
            _spellInfoWriter.AppendFormat("    ReqSkillValue {0}", skill.ReqSkillValue);

            _spellInfoWriter.AppendFormat(", Forward Spell = {0}, MinMaxValue ({1}, {2})", skill.ForwardSpellid, skill.MinValue, skill.MaxValue);
            _spellInfoWriter.AppendFormat(", CharacterPoints ({0}, {1})", skill.CharacterPoints[0], skill.CharacterPoints[1]);
        }

        private void AppendSpellEffectInfo()
        {
            _spellInfoWriter.AppendSeparator();

            for (var effectIndex = 0; effectIndex < DBC.DBC.MaxEffectIndex; effectIndex++)
            {
                _spellInfoWriter.SetBold();
                if ((SpellEffects)_spell.Effect[effectIndex] == SpellEffects.NO_SPELL_EFFECT)
                {
                    _spellInfoWriter.AppendFormatLine("Effect {0}:  NO EFFECT", effectIndex);
                    _spellInfoWriter.AppendLine();
                    continue;
                }

                _spellInfoWriter.AppendFormatLine("Effect {0}: Id {1} ({2})", effectIndex, _spell.Effect[effectIndex], (SpellEffects)_spell.Effect[effectIndex]);
                _spellInfoWriter.SetDefaultStyle();
                _spellInfoWriter.AppendFormat("BasePoints = {0}", _spell.EffectBasePoints[effectIndex] + 1);

                if (_spell.EffectRealPointsPerLevel[effectIndex] != 0)
                    _spellInfoWriter.AppendFormat(" + Level * {0:F}", _spell.EffectRealPointsPerLevel[effectIndex]);

                // WTF ? 1 = spell.EffectBaseDice[i]
                if (1 < _spell.EffectDieSides[effectIndex])
                {
                    if (_spell.EffectRealPointsPerLevel[effectIndex] != 0)
                        _spellInfoWriter.AppendFormat(" to {0} + lvl * {1:F}",
                            _spell.EffectBasePoints[effectIndex] + 1 + _spell.EffectDieSides[effectIndex], _spell.EffectRealPointsPerLevel[effectIndex]);
                    else
                        _spellInfoWriter.AppendFormat(" to {0}", _spell.EffectBasePoints[effectIndex] + 1 + _spell.EffectDieSides[effectIndex]);
                }

                _spellInfoWriter.AppendFormatIfNotNull(" + combo * {0:F}", _spell.EffectPointsPerComboPoint[effectIndex]);

                if (_spell.DmgMultiplier[effectIndex] != 1.0f)
                    _spellInfoWriter.AppendFormat(" x {0:F}", _spell.DmgMultiplier[effectIndex]);

                _spellInfoWriter.AppendFormatIfNotNull("  Multiple = {0:F}", _spell.EffectMultipleValue[effectIndex]);
                _spellInfoWriter.AppendLine();

                _spellInfoWriter.AppendFormatLine("Targets ({0}, {1}) ({2}, {3})",
                    _spell.EffectImplicitTargetA[effectIndex], _spell.EffectImplicitTargetB[effectIndex],
                    (Targets)_spell.EffectImplicitTargetA[effectIndex], (Targets)_spell.EffectImplicitTargetB[effectIndex]);

                AuraModTypeName(effectIndex);

                var classMask = new uint[3];

                switch (effectIndex)
                {
                    case 0: classMask = _spell.EffectSpellClassMaskA; break;
                    case 1: classMask = _spell.EffectSpellClassMaskB; break;
                    case 2: classMask = _spell.EffectSpellClassMaskC; break;
                }

                if (classMask[0] != 0 || classMask[1] != 0 || classMask[2] != 0)
                {
                    _spellInfoWriter.AppendFormatLine("SpellClassMask = {0:X8} {1:X8} {2:X8}", classMask[0], classMask[1], classMask[2]);

                    var query = from spell in DBC.DBC.Spell.Values
                                where spell.SpellFamilyName == _spell.SpellFamilyName && spell.SpellFamilyFlags.ContainsElement(classMask)
                                join sk in DBC.DBC.SkillLineAbility on spell.ID equals sk.Value.SpellId into temp
                                from skill in temp.DefaultIfEmpty()
                                select new
                                {
                                    SpellID = spell.ID,
                                    SpellName = spell.SpellNameRank,
                                    skill.Value.SkillId
                                };

                    foreach (var row in query)
                    {
                        if (row.SkillId > 0)
                        {
                            _spellInfoWriter.SetStyle(Color.Blue, FontStyle.Regular);
                            _spellInfoWriter.AppendFormatLine("\t+ {0} - {1}", row.SpellID, row.SpellName);
                        }
                        else
                        {
                            _spellInfoWriter.SetStyle(Color.Red, FontStyle.Regular);
                            _spellInfoWriter.AppendFormatLine("\t- {0} - {1}", row.SpellID, row.SpellName);
                        }
                        _spellInfoWriter.SetStyle(Color.Black, FontStyle.Regular);
                    }
                }

                _spellInfoWriter.AppendFormatLineIfNotNull("{0}", _spell.GetRadius(effectIndex));

                // append trigger spell
                var trigger = _spell.EffectTriggerSpell[effectIndex];
                if (trigger != 0)
                {
                    if (DBC.DBC.Spell.ContainsKey(trigger))
                    {
                        var triggerSpell = DBC.DBC.Spell[trigger];
                        _spellInfoWriter.SetStyle(Color.Blue, FontStyle.Bold);
                        _spellInfoWriter.AppendFormatLine("   Trigger spell ({0}) {1}. Chance = {2}", trigger, triggerSpell.SpellNameRank, _spell.ProcChance);
                        _spellInfoWriter.AppendFormatLineIfNotNull("   Description: {0}", triggerSpell.Description);
                        _spellInfoWriter.AppendFormatLineIfNotNull("   ToolTip: {0}", triggerSpell.ToolTip);
                        _spellInfoWriter.SetDefaultStyle();
                        if (triggerSpell.ProcFlags != 0)
                        {
                            _spellInfoWriter.AppendFormatLine("Charges - {0}", triggerSpell.ProcCharges);
                            _spellInfoWriter.AppendSeparator();
                            _spellInfoWriter.AppendLine(triggerSpell.ProcInfo);
                            _spellInfoWriter.AppendSeparator();
                        }
                    }
                    else
                    {
                        _spellInfoWriter.AppendFormatLine("Trigger spell ({0}) Not found, Chance = {1}", trigger, _spell.ProcChance);
                    }
                }

                _spellInfoWriter.AppendFormatLineIfNotNull("EffectChainTarget = {0}", _spell.EffectChainTarget[effectIndex]);
                _spellInfoWriter.AppendFormatLineIfNotNull("EffectItemType = {0}", _spell.EffectItemType[effectIndex]);

                if ((Mechanics)_spell.EffectMechanic[effectIndex] != Mechanics.MECHANIC_NONE)
                    _spellInfoWriter.AppendFormatLine("Effect Mechanic = {0} ({1})", _spell.EffectMechanic[effectIndex], (Mechanics)_spell.EffectMechanic[effectIndex]);

                _spellInfoWriter.AppendLine();
            }
        }

        private void AppendSpellAura()
        {
            if (_spell.CasterAuraSpell != 0)
            {
                if (DBC.DBC.Spell.ContainsKey(_spell.CasterAuraSpell))
                    _spellInfoWriter.AppendFormatLine("  Caster Aura Spell ({0}) {1}", _spell.CasterAuraSpell, DBC.DBC.Spell[_spell.CasterAuraSpell].SpellName);
                else
                    _spellInfoWriter.AppendFormatLine("  Caster Aura Spell ({0}) ?????", _spell.CasterAuraSpell);
            }

            if (_spell.TargetAuraSpell != 0)
            {
                if (DBC.DBC.Spell.ContainsKey(_spell.TargetAuraSpell))
                    _spellInfoWriter.AppendFormatLine("  Target Aura Spell ({0}) {1}", _spell.TargetAuraSpell, DBC.DBC.Spell[_spell.TargetAuraSpell].SpellName);
                else
                    _spellInfoWriter.AppendFormatLine("  Target Aura Spell ({0}) ?????", _spell.TargetAuraSpell);
            }

            if (_spell.ExcludeCasterAuraSpell != 0)
            {
                if (DBC.DBC.Spell.ContainsKey(_spell.ExcludeCasterAuraSpell))
                    _spellInfoWriter.AppendFormatLine("  Ex Caster Aura Spell ({0}) {1}", _spell.ExcludeCasterAuraSpell, DBC.DBC.Spell[_spell.ExcludeCasterAuraSpell].SpellName);
                else
                    _spellInfoWriter.AppendFormatLine("  Ex Caster Aura Spell ({0}) ?????", _spell.ExcludeCasterAuraSpell);
            }

            // ReSharper disable InvertIf
            if (_spell.ExcludeTargetAuraSpell != 0)
            {
                if (DBC.DBC.Spell.ContainsKey(_spell.ExcludeTargetAuraSpell))
                    _spellInfoWriter.AppendFormatLine("  Ex Target Aura Spell ({0}) {1}", _spell.ExcludeTargetAuraSpell, DBC.DBC.Spell[_spell.ExcludeTargetAuraSpell].SpellName);
                else
                    _spellInfoWriter.AppendFormatLine("  Ex Target Aura Spell ({0}) ?????", _spell.ExcludeTargetAuraSpell);
            }
            // ReSharper enable InvertIf
        }

        private void AuraModTypeName(int index)
        {
            var aura = (AuraType)_spell.EffectApplyAuraName[index];
            var misc = _spell.EffectMiscValue[index];

            if (_spell.EffectApplyAuraName[index] == 0)
            {
                _spellInfoWriter.AppendFormatLineIfNotNull("EffectMiscValueA = {0}", _spell.EffectMiscValue[index]);
                _spellInfoWriter.AppendFormatLineIfNotNull("EffectMiscValueB = {0}", _spell.EffectMiscValueB[index]);
                _spellInfoWriter.AppendFormatLineIfNotNull("EffectAmplitude = {0}", _spell.EffectAmplitude[index]);

                return;
            }

            _spellInfoWriter.AppendFormat("Aura Id {0:D} ({0})", aura);
            _spellInfoWriter.AppendFormat(", value = {0}", _spell.EffectBasePoints[index] + 1);
            _spellInfoWriter.AppendFormat(", misc = {0} (", misc);

            switch (aura)
            {
                case AuraType.SPELL_AURA_MOD_STAT:
                    _spellInfoWriter.Append((UnitMods)misc);
                    break;
                case AuraType.SPELL_AURA_MOD_RATING:
                    _spellInfoWriter.Append((CombatRating)misc);
                    break;
                case AuraType.SPELL_AURA_ADD_FLAT_MODIFIER:
                case AuraType.SPELL_AURA_ADD_PCT_MODIFIER:
                    _spellInfoWriter.Append((SpellModOp)misc);
                    break;
                // todo: more case
                default:
                    _spellInfoWriter.Append(misc);
                    break;
            }

            _spellInfoWriter.AppendFormat("), miscB = {0}", _spell.EffectMiscValueB[index]);
            _spellInfoWriter.AppendFormatLine(", periodic = {0}", _spell.EffectAmplitude[index]);

            switch (aura)
            {
                case AuraType.SPELL_AURA_OVERRIDE_SPELLS:
                    if (!DBC.DBC.OverrideSpellData.ContainsKey((uint)misc))
                    {
                        _spellInfoWriter.SetStyle(Color.Red, FontStyle.Bold);
                        _spellInfoWriter.AppendFormatLine("Cannot find key {0} in OverrideSpellData.dbc", (uint)misc);
                    }
                    else
                    {
                        _spellInfoWriter.AppendLine();
                        _spellInfoWriter.SetStyle(Color.DarkRed, FontStyle.Bold);
                        _spellInfoWriter.AppendLine("Overriding Spells:");
                        var @override = DBC.DBC.OverrideSpellData[(uint)misc];
                        for (var i = 0; i < 10; ++i)
                        {
                            if (@override.Spells[i] == 0)
                                continue;

                            _spellInfoWriter.SetStyle(Color.DarkBlue, FontStyle.Regular);
                            _spellInfoWriter.AppendFormatLine("\t - #{0} ({1}) {2}", i + 1, @override.Spells[i],
                                DBC.DBC.Spell.ContainsKey(@override.Spells[i]) ? DBC.DBC.Spell[@override.Spells[i]].SpellName : "?????");
                        }
                        _spellInfoWriter.AppendLine();
                    }
                    break;
                case AuraType.SPELL_AURA_SCREEN_EFFECT:
                    _spellInfoWriter.SetStyle(Color.DarkBlue, FontStyle.Bold);
                    _spellInfoWriter.AppendFormatLine("ScreenEffect: {0}",
                        DBC.DBC.ScreenEffect.ContainsKey((uint)misc) ? DBC.DBC.ScreenEffect[(uint)misc].Name : "?????");
                    break;
            }
        }

        private void AppendDifficultyInfo()
        {
            var difficultyId = _spell.SpellDifficultyId;
            if (difficultyId == 0)
                return;

            if (!DBC.DBC.SpellDifficulty.ContainsKey(difficultyId))
            {
                _spellInfoWriter.AppendFormatLine("Cannot find difficulty overrides for id {0} in SpellDifficulty.dbc!", difficultyId);
                return;
            }

            var modeNames = new[]
            {
                "Normal 10",
                "Normal 25",
                "Heroic 10",
                "Heroic 25",
            };

            _spellInfoWriter.SetBold();
            _spellInfoWriter.AppendLine("Spell difficulty Ids:");

            var difficulty = DBC.DBC.SpellDifficulty[difficultyId];
            for (var i = 0; i < 4; ++i)
            {
                if (difficulty.SpellId[i] <= 0)
                    continue;

                _spellInfoWriter.AppendFormatLine("{0}: {1}", modeNames[i], difficulty.SpellId[i]);
            }
        }

        private void AppendAreaInfo()
        {
            if (_spell.AreaGroupId <= 0)
                return;

            var areaGroupId = (uint)_spell.AreaGroupId;
            if (!DBC.DBC.AreaGroup.ContainsKey(areaGroupId))
            {
                _spellInfoWriter.AppendFormatLine("Cannot find area group id {0} in AreaGroup.dbc!", _spell.AreaGroupId);
                return;
            }

            _spellInfoWriter.AppendLine();
            _spellInfoWriter.SetBold();
            _spellInfoWriter.AppendLine("Allowed areas:");
            while (DBC.DBC.AreaGroup.ContainsKey(areaGroupId))
            {
                var groupEntry = DBC.DBC.AreaGroup[areaGroupId];
                for (var i = 0; i < 6; ++i)
                {
                    var areaId = groupEntry.AreaId[i];
                    if (DBC.DBC.AreaTable.ContainsKey(areaId))
                    {
                        var areaEntry = DBC.DBC.AreaTable[areaId];
                        _spellInfoWriter.AppendFormatLine("{0} - {1} (Map: {2})", areaId, areaEntry.Name, areaEntry.MapId);
                    }
                }


                if (groupEntry.NextGroup == 0)
                    break;

                // Try search in next group
                areaGroupId = groupEntry.NextGroup;
            }

            _spellInfoWriter.AppendLine();
        }
    }
}