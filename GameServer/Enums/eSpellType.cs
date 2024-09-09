namespace DOL.GS
{
    public enum eSpellType : short
    {
        None,
        SavageCombatSpeedBuff,
        SavageEvadeBuff,
        SavageCrushResistanceBuff,
        SavageThrustResistanceBuff,
        BaseArmorFactorBuff,
        SpecArmorFactorBuff,
        PaladinArmorFactorBuff,
        HealthRegenBuff,
        SummonTheurgistPet,
        CombatHeal,
        DamageAdd,
        Resurrect,
        BodyResistBuff,
        SpiritResistBuff,
        EnergyResistBuff,
        HeatResistBuff,
        ColdResistBuff,
        MatterResistBuff,
        Taunt,
        EnduranceRegenBuff,
        HeatColdMatterBuff,
        BodySpiritEnergyBuff,
        SpeedEnhancement,
        Lifedrain,
        LifedrainNoVariance,
        DamageShield,
        Bladeturn,
        DirectDamage,
        SpeedDecrease,
        SummonAnimistPet,
        Bomber,
        SummonAnimistFnFCustom,
        DamageOverTime,
        DamageOverTimeNoVariance,
        PowerRegenBuff,
        CombatSpeedBuff,
        ArmorAbsorptionBuff,
        OffensiveProc,
        DefensiveProc,
        ConstitutionDebuff,
        DirectDamageWithDebuff,
        DirectDamageWithDebuffNoVariance,
        MeleeDamageDebuff,
        ArmorAbsorptionDebuff,
        Charm,
        Heal,
        Stun,
        AblativeArmor,
        TurretPBAoE,
        SummonAnimistFnF,
        TurretsRelease,
        Mesmerize,
        Confusion,
        CureMezz,
        CombatSpeedDebuff,
        DamageSpeedDecrease,
        CurePoison,
        CureDisease,
        CureNearsightCustom,
        SpreadHeal,
        StrengthBuff,
        DexterityBuff,
        ConstitutionBuff,
        StrengthConstitutionBuff,
        DexterityQuicknessBuff,
        AcuityBuff,
        MeleeDamageBuff,
        FatigueConsumptionBuff,
        ArcheryDoT,
        SavageDPSBuff,
        StyleTaunt,
        StyleCombatSpeedDebuff,
        StrengthDebuff,
        SlashResistDebuff,
        StyleBleeding,
        StyleSpeedDecrease,
        StyleStun,
        SavageSlashResistanceBuff,
        SpeedOfTheRealm,
        DexterityDebuff,
        DexterityQuicknessDebuff,
        BodyResistDebuff,
        SpiritResistDebuff,
        EnergyResistDebuff,
        Nearsight,
        StrengthConstitutionDebuff,
        LifeTransfer,
        SummonSpiritFighter,
        MesmerizeDurationBuff,
        Bolt,
        HeatResistDebuff,
        ColdResistDebuff,
        MatterResistDebuff,
        ArmorFactorDebuff,
        HealOverTime,
        Amnesia,
        Disease,
        SummonHunterPet,
        SummonUnderhill,
        HereticPiercingMagic,
        SummonDruidPet,
        NightshadeNuke,
        SavageParryBuff,
        SavageEnduranceHeal,
        ArrowDamageTypes,
        Archery,
        SiegeArrow,
        SummonSimulacrum,
        PetConversion,
        PetSpell,
        PowerDrainPet,
        PowerTransferPet,
        FacilitatePainworking,
        SummonNecroPet,
        SummonNoveltyPet,
        EternalPlant,
        PowerHealthEnduranceRegenBuff,
        GatewayPersonalBind,
        UniPortal,
        FrontalPulseConeDD,

        //--------------------Added when refactoring--------------------
        MercHeal,
        OmniHeal,
        PBAoEHeal,
        SummonHealingElemental,
        Pet, // May not be needed.
        AFHitsBuff,
        AllMagicResistBuff,
        Buff,
        CelerityBuff,
        CourageBuff,
        CrushSlashTrustBuff,
        EffectivenessBuff,
        FlexibleSkillBuff,
        HasteBuff,
        HeroismBuff,
        KeepDamageBuff,
        MLABSBuff,
        ParryBuff,
        MagicResistBuff,
        SuperiorCourageBuff,
        ToHitBuff,
        WeaponSkillBuff,
        Summon,
        SummonMinion,
        SummonCommander,
        AllStatsPercentDebuff,
        CrushSlashThrustDebuff,
        EffectivenessDebuff,
        Mez,
        StyleHandler,
        MLStyleHandler,
        BodyguardHandler,
        Chamber,
        Morph,
        MagicalStrike,
        UnrresistableNonImunityStun,
        Disarm,
        Uninterruptable,
        Powerless,
        Range,
        Prescience,
        PowerRend,
        SpeedWrap,
        FumbleChanceDebuff,
        SiegeDirectDamage,
        DirectDamageNoVariance,
        PowerOverTime,
        PoisonspikeDot,
        UnresistableStun,
        BloodRage,
        HeightenedAwareness,
        SubtleKills,
        StormMissHit,
        StormEnduDrain,
        StormDexQuickDebuff,
        PowerDrainStorm,
        StormStrConstDebuff,
        StormAcuityDebuff,
        StormEnergyTempest,
        DamageSpeedDecreaseNoVariance,
        PetLifedrain,
        Phaseshift,
        Grapple,
        BrittleGuard,
        StyleRange,
        MultiTarget,
        PiercingMagic,
        PveResurrectionIllness,
        RvrResurrectionIllness,
        UnbreakableSpeedDecrease,
        WeaponSkillConstitutionDebuff,
        EnduranceHeal,
        PowerHeal,
        FatigueConsumptionDebuff,
        NaturesShield,
        AllStatsBarrel,
        DexterityConstitutionDebuff,
        ComfortingFlames,
        AllRegenBuff,
        SummonMerchant,
        BeadRegen,
        SummonVaultkeeper,
        OffhandDamage,
        OffhandChance,
        SummonSiegeRam,
        SummonSiegeBallista,
        SummonSiegeCatapult,
        SummonSiegeTrebuchet,
        SummonJuggernaut,
        SummonAnimistAmbusher,
        StrikingTheSoul,
        EnduranceDrain,
        VampiirArmorDebuff,
        VampiirBolt,
        VampiirEffectivenessDeBuff,
        VampiirMagicResistance,
        VampiirMeleeResistance,
        VampiirSkillBonusDeBuff,
        VampiirStealthDetection,
        VampSpeedDecrease,
        ABSDamageShield,
        RampingDamageFocus,
        StatDebuffReturn,
        AllMagicResistsBuff,
        AllSecondaryMagicResistsBuff,
        WaterBreathing,
        WarlockSpeedDecrease,
        BainsheePulseDmg,
        Fear,
        Climbing,
        Traldor,
        TraitorsDaggerSummon,
        TraitorsDaggerProc,
        ToHitDebuff,
        ThrustResistDebuff,
        BeFriend,
        BothAblativeArmor,
        Silence,
        CombineScrolls,
        ZahurHealProc,
        MagicAblativeArmor,
        ZahurAura,
        Conversion,
        TargetModifier,
        Tartaros,
        Costume,
        BeltOfLight,
        OmniLifedrain,
        SummonTrebuchet,
        Call,
        CrushSlashThrustBuff,
        FOH,
        SummonSiegeWeapon,
        SummonSalamander,
        SummonMonster,
        MonsterDoT,
        RandomBuffShear,
        StyleDmgAbs,
        ReanimateCorpse,
        AcuityDebuff,
        AcuityShear,
        AllStatsDebuff,
        AlvarusMorph,
        AncientTransmuter,
        ArcaneLeadership,
        Arrogance,
        AtensShield,
        Banespike,
        Battlewarder,
        Bedazzlement,
        BeltOfMoon,
        BlanketOfCamouflage,
        BolsteringRoar,
        CalmingNotes,
        CastingSpeedDebuff,
        CeremonialBracerMezz,
        CeremonialBracerStun,
        ChokingVapors,
        CleansingAura,
        CloudsongFall,
        ConstitutionShear,
        CoweringBellow,
        Critical,
        CrocodileTearsProc,
        CrushResistDebuff,
        CureAll,
        CureNearsight,
        DamageToPower,
        DazzlingArray,
        Decoy,
        DexterityQuicknessShear,
        DexterityShear,
        DissonanceTrap,
        DoomHammer,
        DreamGroupMorph,
        DreamMorph,
        EnergyTempest,
        EnervatingGas,
        EssenceFlare,
        EssenceResist,
        EssenceSear,
        EvadeBuff,
        ExtraHP,
        FocusingWinds,
        FocusShell,
        FOD,
        FOP,
        FOR,
        Fury,
        GoldenSpearJavelin,
        GoVAF,
        Groupport,
        GroupRecall,
        HealthToEndurance,
        HpPwrEndRegen,
        IllusionSpell,
        IllusionBladeSummon,
        InebriatingFumes,
        Lookout,
        MaddeningScalars,
        MentalSiphon,
        MetalGuard,
        MissHit,
        MLEndudrain,
        MLFatDebuff,
        MLManadrain,
        MLUnbreakableSnare,
        NearsightReduction,
        OffensiveProcPvE,
        Oppression,
        PassiveSpell,
        PBAEDamage,
        PBAEHeal,
        PoisonSpike,
        Port,
        PortableFreyadHelper,
        PowerDrain, 
        PowerTransfer,
        PowerTrap,
        PrescienceNode,
        QuicknessDebuff,
        Rampage,
        RangeShield,
        RealmLore,
        Recall,
        Sabotage,
        ScarabProc,
        SenseDullingCloud,
        ShadesOfMist,
        SickHeal,
        SiegeWrecker,
        SnakeCharmer,
        SpeedWrapWard,
        StarsProc,
        StarsProc2,
        StrengthConstitutionShear,
        StrengthShear,
        SummonElemental,
        SummonMastery,
        SummonRam,
        SummonTotem,
        SummonWarcrystal,
        SummonWood,
        TangleSnare,
        ThrowWeapon,
        UnmakeCrystalseed,
        UnyeldingProc,
        VacuumVortex,
        YouthIllness,
        ZahurEnduProc,
        ZahurPowerTransfer,
        Zephyr,
        ZoAura,
        ZoSummon,
        ChainBolt,
        AfHitsBuff,
        AtlantisTabletMorph,
        BeltOfSun,
        CloudsongAura,
        LoreDebuff,
        StrengthConstitutionDrain,
        EfficientHealing,
        EfficientEndurance,
        Powershield,
        ShatterIllusions,
        HealFlask,
        DeadFlask,
        HarpyFeatherCloak,
        DdtProcDd,
        AstralPetSummon,
        DexStrConQuiTap,
        ArmorReducingEffectiveness,
        MagicConversion,
        DmgReductionAndPowerReturn,
        Facilis,
        HereticDamageSpeedDecrease,
        HereticDamageOverTime,
        HereticSpeedDecrease,
        MonsterDisease,
        BLToHit,
        EssenceFlamesProc,
        EssenceSearHandler,
        EssenceDampenHandler,
        SummonTitan,
        CCResist,
        Loockout,
        PetMesmerize,
        AllMeleeResistsBuff,
        CrushResistBuff,
        SlashResistBuff,
        ThrustResistBuff,
        AllResistsBuff,
        ArmorFactorBuff,
        QuicknessBuff,
        DPSBuff,
        StealthSkillBuff,
        MagicResistsBuff,
        StyleAbsorbBuff,
        ResiPierceBuff,
        DPSDebuff,
        SkillsDebuff,
        StylePowerDrain,
        UniPortalKeep,
        ValkyrieOffensiveProc,
        BuffCommand
    }
}
