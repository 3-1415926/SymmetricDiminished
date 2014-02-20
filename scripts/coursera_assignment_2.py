from scales import get_scale, draw_scale, Note, guitar


draw_scale(
    guitar,
    get_scale(
        [Note.C, Note.D, Note.E, Note.Fsharp_Gflat, Note.G, Note.A, Note.Asharp_Bflat],
        Note.A),
    "guitar_a_lydian_flat7.png")

draw_scale(
    guitar,
    get_scale(
        [Note.B, Note.C, Note.D, Note.E, Note.F, Note.G, Note.A], Note.C),
    "guitar_c_locrian.png")

draw_scale(
    guitar,
    get_scale(
        [Note.F, Note.Fsharp_Gflat, Note.Gsharp_Aflat, Note.A, Note.B, Note.Csharp_Dflat, Note.Dsharp_Eflat]),
    "guitar_f7_altered.png")

draw_scale(
    guitar,
    get_scale(
        [Note.C, Note.D, Note.Dsharp_Eflat, Note.F, Note.G, Note.A, Note.Asharp_Bflat],
        Note.D),
    "guitar_d_dorian.png")

draw_scale(
    guitar,
    get_scale(
        [Note.C, Note.D, Note.E, Note.Fsharp_Gflat, Note.G, Note.A, Note.B],
        Note.B),
    "guitar_b_lydian.png")

draw_scale(
    guitar,
    get_scale(
        [Note.A, Note.B, Note.C, Note.D, Note.E, Note.F, Note.G],
        Note.Dsharp_Eflat),
    "guitar_eb_aeolian.png")
