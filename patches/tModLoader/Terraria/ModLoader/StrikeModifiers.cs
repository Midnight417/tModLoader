﻿using System;

namespace Terraria.ModLoader;

public struct StrikeModifiers
{
	public static StrikeModifiers Default = new() {
		SourceDamage = StatModifier.Default,
		Defense = StatModifier.Default,
		DefenseEffectiveness = MultipliableFloat.Default * .5f,
		DamageVariationScale = MultipliableFloat.Default,
		CritDamage = StatModifier.Default + 1f,
		FinalDamage = StatModifier.Default,
		Knockback = StatModifier.Default,
	};

	/// <summary>
	/// Use this to enhance or scale the base damage of the item/projectile/hit. This damage modifier will apply to <see cref="Strike.SourceDamage"/> and be transferred to on-hit effects. <br/>
	/// <br/>
	/// For effects which apply to all damage dealt by the player, or a specific damage type, consider using <see cref="Player.GetDamage"/> instead. <br/>
	/// For effects which apply to all dealt by an item, consider using <see cref="GlobalItem.ModifyWeaponDamage"/> instead. <br/>
	/// <br/>
	/// Used by vanilla for weapons with unique scaling such as jousting lance, ham bat, breaker blade. And for accessories which enhance a projectile (strong bees)
	/// </summary>
	public StatModifier SourceDamage;

	/// <summary>
	/// Use this to add bonus damage to the strike, but not to on-hit effects. <br/>
	/// <br/>
	/// Used by vanilla for most summon tag damage.
	/// </summary>
	public AddableFloat FlatBonusDamage;

	/// <summary>
	/// Use this to add bonus <br/>
	/// Used by vanilla for melee parry buff (+4f) and some summon tag damage.
	/// </summary>
	public AddableFloat ScalingBonusDamage;

	/// <summary>
	/// Not recommended for modded use, consider multiplying <see cref="FinalDamage"/> instead. <br/>
	/// Used by vanilla for banners, cultist projectile resistances, extra damage for stakes against vampires etc.
	/// </summary>
	public MultipliableFloat TargetDamageMultiplier;

	/// <summary>
	/// The defense of the receiver, including any temporary modifiers (buffs/debuffs). <br/>
	/// <br/>
	/// Increase <see cref="StatModifier.Base"/> to add extra defense. <br/>
	/// Add for scaling buffs (eg +0.1f for +10% defense). <br/>
	/// Multiply for debuffs (eg *0.9f for -10% defense). <br/>
	/// Decrease <see cref="StatModifier.Flat"/> to provide flat debuffs like ichor or betsys curse <br/>
	/// </summary>
	public StatModifier Defense;

	/// <summary>
	/// Flat defense reduction. Applies after <see cref="ScalingArmorPenetration"/>. <br/>
	/// Add to give bonus flat armor penetration. <br/>
	/// Do not subtract or multiply, consider altering <see cref="Defense"/> or <see cref="ScalingArmorPenetration"/> instead.
	/// <br/>
	/// Used by the <see cref="Projectile.ArmorPenetration"/>, <see cref="Item.ArmorPenetration"/> and <see cref="Player.GetTotalArmorPenetration"/> stats.
	/// </summary>
	public AddableFloat ArmorPenetration;

	/// <summary>
	/// Used to ignore a fraction of enemy armor. Applies before flat <see cref="ArmorPenetration"/>. <br/>
	/// Recommend only additive buffs, no multiplication or subtraction. <br/>
	/// <br/>
	/// At 1f, the attack will completely ignore all defense.
	/// </summary>
	public AddableFloat ScalingArmorPenetration;

	/// <summary>
	/// The conversion ratio between defense and damage reduction. Defaults to 0.5 for NPCs. Depends on difficulty for players. <br/>
	/// Increase to make defense more effective and armor penetration more important. <br/>
	/// <br/>
	/// Recommend only multiplication, no addition or subtraction. <br/>
	/// Not recommended to for buffs/debuffs. Use for gamemode tweaks, or if an enemy revolves very heavily around armor penetration.
	/// </summary>
	public MultipliableFloat DefenseEffectiveness;

	/// <summary>
	/// Applied to the final damage (after defense) result when the strike is a crit. Defaults to +1f additive (+100% damage). <br/>
	///  <br/>
	/// Add to give strikes extra crit damage (eg +0.1f for 10% extra crit damage (total +110% or 2.1 times base). <br/>
	/// Add to <see cref="StatModifier.Flat"/> to give crits extra flat damage. Use with caution as this extra damage will not be reduced by armor. <br/>
	/// Multiplication not recommended for buffs. Could be used to decrease the effectiveness of crits on an enemy without disabling completely. <br/>
	/// Use of <see cref="StatModifier.Base"/> also not recommended. <br/>
	/// </summary>
	public StatModifier CritDamage;

	/// <summary>
	/// Applied to the final damage result. <br/>
	/// Used by <see cref="NPC.takenDamageMultiplier"/> to make enemies extra susceptible/resistant to damage. <br/>
	/// <br/>
	/// Multiply to make your enemy more susceptible or resistant to damage. <br/>
	/// Add to give 'bonus' post-mitigation damage. <br/>
	/// Adding to <see cref="StatModifier.Flat"/> will grant unconditional bonus damage, ignoring all resistances or multipliers. <br/>
	/// </summary>
	public StatModifier FinalDamage;

	/// <summary>
	/// Multiply to adjust the damage variation of the strike. <br/>
	/// Multiply by 0 to disable damage variation.<br/>
	/// Default damage variation is 15%, so maximum scale is ~6.67 <br/>
	/// Only affects strikes where damage variation is enabled (which is most projectile/item/NPC damage)
	/// </summary>
	public MultipliableFloat DamageVariationScale;

	private bool? _critOverride;

	/// <summary>
	/// Disables <see cref="CritDamage"/> calculations, and clears <see cref="Strike.Crit"/> flag from the resulting strike.
	/// </summary>
	public void DisableCrit() => _critOverride = false;

	/// <summary>
	/// Sets the strike to be a crit. Does nothing if <see cref="DisableCrit"/> has been called
	/// </summary>
	public void SetCrit() => _critOverride ??= true;

	/// <summary>
	/// Used by <see cref="NPC.onFire2"/> buff (additive) and <see cref="NPC.knockBackResist"/> (multiplicative) <br/>
	/// <br/>
	/// Recommend using <see cref="GlobalItem.ModifyWeaponKnockback"/> or <see cref="Player.GetKnockback"/> instead where possible.<br/>
	/// <br/>
	/// Add for knockback buffs. <br/>
	/// Multiply for knockback resistances. <br/>
	/// Subtraction not recommended. <br/>
	/// <br/>
	/// Knockback falloff still applies after this, so high knockback has diminishing returns. <br/>
	/// </summary>
	public StatModifier Knockback;

	/// <summary>
	/// Overrides the default hit direction logic. <br/>
	/// If set by multiple mods, only the last override will apply. <br/>
	/// Not recommended for use outside <see cref="ModProjectile.ModifyHit"/>
	/// </summary>
	public int? HitDirectionOverride;

	private bool _instantKill;
	/// <summary>
	/// Set to make the strike instantly kill the target, dealing as much damage as necessary. </br>
	/// Combat text will not be shown.
	/// </summary>
	public void SetInstantKill() => _instantKill = true;

	private bool _combatTextHidden;
	/// <summary>
	/// Set to hide the damage number popup for this strike.
	/// </summary>
	public void HideCombatText() => _combatTextHidden = true;

	/// <summary>
	/// Used internally for calculating the equivalent vanilla strike damage for networking with vanilla clients
	/// </summary>
	private float _calculatedPostDefenseDamage;

	public int GetDamage(float baseDamage, bool crit, bool damageVariation = false, float luck = 0f)
	{
		float damage = SourceDamage.ApplyTo(baseDamage);
		damage += FlatBonusDamage.Value + ScalingBonusDamage.Value * damage;
		damage *= TargetDamageMultiplier.Value;

		int variationPercent = Utils.Clamp((int)Math.Round(Main.DefaultDamageVariationPercent * DamageVariationScale.Value), 0, 100);
		if (damageVariation && variationPercent > 0)
			damage = Main.DamageVar(damage, variationPercent, luck);

		float defense = Defense.ApplyTo(0);
		float armorPenetration = defense * Math.Clamp(ScalingArmorPenetration.Value, 0, 1) + ArmorPenetration.Value;
		defense = Math.Max(defense - armorPenetration, 0);

		float damageReduction = defense * DefenseEffectiveness.Value;
		damage = Math.Max(damage - damageReduction, 1);
		_calculatedPostDefenseDamage = damage;

		if (_critOverride ?? crit)
			damage = CritDamage.ApplyTo(damage);

		return Math.Max((int)FinalDamage.ApplyTo(damage), 1);
	}

	public float GetKnockback(float baseKnockback) => Math.Max(Knockback.ApplyTo(baseKnockback), 0);

	internal int GetVanillaDamage(int targetDefense) => (int)(_calculatedPostDefenseDamage + targetDefense / 2);

	public Strike ToStrike(DamageClass damageType, float baseDamage, bool crit, float baseKnockback, int hitDirection, bool damageVariation = false, float luck = 0f) => new() {
		DamageType = damageType,
		SourceDamage = Math.Max((int) SourceDamage.ApplyTo(baseDamage), 1),
		Damage = _instantKill ? 0 : GetDamage(baseDamage, crit, damageVariation, luck),
		Crit = _critOverride ?? crit,
		KnockBack = GetKnockback(baseKnockback),
		HitDirection = HitDirectionOverride ?? hitDirection,
		InstantKill = _instantKill,
		HideCombatText = _combatTextHidden
	};
}
