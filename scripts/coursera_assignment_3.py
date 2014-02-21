from scales import get_scale, draw_scale, Note, guitar

AEOLIAN = [Note.E, Note.Fsharp_Gflat, Note.G, Note.A, Note.B, Note.C, Note.D]
LOCRIAN = [Note.B, Note.C, Note.D, Note.E, Note.F, Note.G, Note.A]

ALTERED = [Note.F, Note.Fsharp_Gflat, Note.Gsharp_Aflat, Note.A, Note.B, Note.Csharp_Dflat, Note.Dsharp_Eflat]

DORIAN = [Note.C, Note.D, Note.Dsharp_Eflat, Note.F, Note.G, Note.A, Note.Asharp_Bflat]

LYDIAN = [Note.C, Note.D, Note.E, Note.Fsharp_Gflat, Note.G, Note.A, Note.B]


draw_scale(
    guitar,
    get_scale(AEOLIAN, Note.E),
    "01_guitar_e_aeolian.png")

draw_scale(
    guitar,
    get_scale(DORIAN, Note.G),
    "02_guitar_g_dorian.png")

draw_scale(
    guitar,
    get_scale(LYDIAN, Note.Asharp_Bflat),
    "03_guitar_b_flat_lydian.png")

draw_scale(
    guitar,
    get_scale(LOCRIAN, Note.B),
    "04_guitar_b_locrian.png")

draw_scale(
    guitar,
    get_scale(ALTERED, Note.E),
    "05_guitar_e_altered.png")

draw_scale(
    guitar,
    get_scale(AEOLIAN, Note.A),
    "06_guitar_a_aeolian.png")

draw_scale(
    guitar,
    get_scale(LOCRIAN, Note.Fsharp_Gflat),
    "07_guitar_f_sharp_locrian.png")

draw_scale(
    guitar,
    get_scale(DORIAN, Note.F),
    "08_guitar_f_dorian.png")

draw_scale(
    guitar,
    get_scale(AEOLIAN, Note.C),
    "09_guitar_c_aeolian.png")

draw_scale(
    guitar,
    get_scale(ALTERED, Note.B),
    "10_guitar_b_altered.png")
