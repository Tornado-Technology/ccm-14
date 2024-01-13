ent-CMBaseDefibrillator = defibrillator
    .desc = CLEAR! Zzzzat!
ent-CMDefibrillator = { ent-['CMBaseDefibrillator', 'PowerCellSlotMediumItem'] }

  .suffix = CM14
  .desc = { ent-['CMBaseDefibrillator', 'PowerCellSlotMediumItem'].desc }
ent-CMDefibrillatorEmpty = { ent-CMDefibrillator }
    .suffix = CM14, Empty
    .desc = { ent-CMDefibrillator.desc }
ent-CMDefibrillatorInfinite = { ent-CMBaseDefibrillator }
    .suffix = CM14, Infinite
    .desc = { ent-CMBaseDefibrillator.desc }
