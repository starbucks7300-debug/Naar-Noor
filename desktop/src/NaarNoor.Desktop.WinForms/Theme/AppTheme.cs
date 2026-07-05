namespace NaarNoor.Desktop.WinForms.Theme;

public static class AppTheme
{
    // ── Brand ────────────────────────────────────────────
    public static readonly Color Primary      = Color.FromArgb(198, 90,  30);
    public static readonly Color PrimaryHover = Color.FromArgb(217, 114, 64);
    public static readonly Color PrimaryDark  = Color.FromArgb(160, 74,  24);

    // ── Danger / Success ─────────────────────────────────
    public static readonly Color Danger  = Color.FromArgb(239, 68,  68);
    public static readonly Color Success = Color.FromArgb( 16, 185, 129);

    // ── Dark surfaces ────────────────────────────────────
    public static readonly Color Background          = Color.FromArgb( 10,  10,  10);
    public static readonly Color BackgroundSecondary = Color.FromArgb( 13,  13,  13);
    public static readonly Color Surface             = Color.FromArgb( 17,  17,  17);
    public static readonly Color SurfaceHover        = Color.FromArgb( 26,  26,  26);
    public static readonly Color TopBar              = Color.FromArgb( 17,  17,  17);
    public static readonly Color Sidebar             = Color.FromArgb( 13,  13,  13);

    // ── Text ─────────────────────────────────────────────
    public static readonly Color TextPrimary   = Color.FromArgb(255, 255, 255);
    public static readonly Color TextSecondary = Color.FromArgb(115, 115, 115);
    public static readonly Color TextMuted     = Color.FromArgb( 82,  82,  82);

    // ── Borders ──────────────────────────────────────────
    public static readonly Color Border       = Color.FromArgb( 30,  30,  30);
    public static readonly Color BorderStrong = Color.FromArgb( 50,  50,  50);

    // ── Input / Control backgrounds ──────────────────────
    public static readonly Color InputBackground = Color.FromArgb( 20,  20,  20);

    // ── Fonts ────────────────────────────────────────────
    public static readonly Font FontHeading  = new("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font FontSubhead  = new("Segoe UI", 11f, FontStyle.Bold);
    public static readonly Font FontBody     = new("Segoe UI",  9f, FontStyle.Regular);
    public static readonly Font FontBodyBold = new("Segoe UI",  9f, FontStyle.Bold);
    public static readonly Font FontSmall    = new("Segoe UI",  8f, FontStyle.Regular);
    public static readonly Font FontLabel    = new("Segoe UI",  8f, FontStyle.Regular);

    // ── Grid columns ─────────────────────────────────────
    public static readonly Color GridHeader   = Color.FromArgb( 17,  17,  17);
    public static readonly Color GridRow      = Color.FromArgb( 13,  13,  13);
    public static readonly Color GridAlt      = Color.FromArgb( 10,  10,  10);
    public static readonly Color GridSelected = Color.FromArgb(198,  90,  30);
    public static readonly Color GridLine     = Color.FromArgb( 30,  30,  30);
}
