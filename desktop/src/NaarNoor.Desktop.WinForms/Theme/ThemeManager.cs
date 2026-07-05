namespace NaarNoor.Desktop.WinForms.Theme;

public static class ThemeManager
{
    public static void Apply(Form form)
    {
        form.BackColor = AppTheme.Background;
        form.ForeColor = AppTheme.TextPrimary;
        form.Font      = AppTheme.FontBody;

        ApplyToControls(form.Controls);
    }

    private static void ApplyToControls(Control.ControlCollection controls)
    {
        foreach (Control c in controls)
        {
            ApplyControl(c);
            if (c.HasChildren)
                ApplyToControls(c.Controls);
        }
    }

    private static void ApplyControl(Control c)
    {
        switch (c)
        {
            case Panel panel:
                ApplyPanel(panel);
                break;

            case Label label:
                label.BackColor = Color.Transparent;
                label.ForeColor = IsSubHeading(label) ? AppTheme.TextPrimary : AppTheme.TextSecondary;
                break;

            case Button btn:
                ApplyButton(btn);
                break;

            case TextBox tb:
                tb.BackColor   = AppTheme.InputBackground;
                tb.ForeColor   = AppTheme.TextPrimary;
                tb.BorderStyle = BorderStyle.FixedSingle;
                break;

            case RichTextBox rtb:
                rtb.BackColor = AppTheme.InputBackground;
                rtb.ForeColor = AppTheme.TextPrimary;
                break;

            case ComboBox cmb:
                cmb.BackColor      = AppTheme.InputBackground;
                cmb.ForeColor      = AppTheme.TextPrimary;
                cmb.FlatStyle      = FlatStyle.Flat;
                break;

            case NumericUpDown nud:
                nud.BackColor = AppTheme.InputBackground;
                nud.ForeColor = AppTheme.TextPrimary;
                break;

            case DateTimePicker dtp:
                dtp.CalendarMonthBackground = AppTheme.Surface;
                dtp.CalendarForeColor       = AppTheme.TextPrimary;
                dtp.CalendarTitleBackColor  = AppTheme.Primary;
                dtp.CalendarTitleForeColor  = AppTheme.TextPrimary;
                break;

            case TabControl tab:
                ApplyTabControl(tab);
                break;

            case DataGridView dgv:
                ApplyDataGrid(dgv);
                break;

            case GroupBox gb:
                gb.BackColor = AppTheme.Surface;
                gb.ForeColor = AppTheme.TextSecondary;
                break;

            case CheckBox chk:
                chk.BackColor = Color.Transparent;
                chk.ForeColor = AppTheme.TextSecondary;
                break;

            case ProgressBar pb:
                pb.BackColor  = AppTheme.Surface;
                pb.ForeColor  = AppTheme.Primary;
                break;

            case ListBox lb:
                lb.BackColor = AppTheme.InputBackground;
                lb.ForeColor = AppTheme.TextPrimary;
                break;

            default:
                if (c.BackColor != Color.Transparent)
                    c.BackColor = AppTheme.Background;
                c.ForeColor = AppTheme.TextSecondary;
                break;
        }
    }

    private static void ApplyPanel(Panel panel)
    {
        var tag = panel.Tag?.ToString() ?? "";

        panel.BackColor = tag switch
        {
            "topbar"   => AppTheme.TopBar,
            "sidebar"  => AppTheme.Sidebar,
            "surface"  => AppTheme.Surface,
            "card"     => AppTheme.Surface,
            "filter"   => AppTheme.Surface,
            _          => panel.Dock == DockStyle.Top    ? AppTheme.TopBar
                        : panel.Dock == DockStyle.Left   ? AppTheme.Sidebar
                        : AppTheme.Background,
        };
    }

    private static void ApplyButton(Button btn)
    {
        var text = btn.Text.Trim().ToLowerInvariant();
        var tag  = btn.Tag?.ToString()?.ToLowerInvariant() ?? "";

        bool isDanger  = tag == "danger"  || text is "logout" or "delete" or "cancel";
        bool isPrimary = tag == "primary" || text is "login" or "save" or "create" or "refresh"
                                          || text.Contains("new") || text.Contains("add");

        if (isDanger)
        {
            btn.BackColor = AppTheme.Danger;
            btn.ForeColor = Color.White;
        }
        else if (isPrimary)
        {
            btn.BackColor = AppTheme.Primary;
            btn.ForeColor = Color.White;
        }
        else
        {
            btn.BackColor = AppTheme.Surface;
            btn.ForeColor = AppTheme.TextSecondary;
        }

        btn.FlatStyle                    = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize    = 0;
        btn.FlatAppearance.MouseOverBackColor = isDanger ? AppTheme.Danger
                                              : isPrimary ? AppTheme.PrimaryHover
                                              : AppTheme.SurfaceHover;
        btn.Cursor = Cursors.Hand;
    }

    private static void ApplyTabControl(TabControl tab)
    {
        tab.BackColor  = AppTheme.Background;
        tab.ForeColor  = AppTheme.TextPrimary;
        tab.Padding    = new Point(16, 8);
    }

    private static void ApplyDataGrid(DataGridView dgv)
    {
        dgv.BackgroundColor              = AppTheme.Background;
        dgv.GridColor                    = AppTheme.GridLine;
        dgv.DefaultCellStyle.BackColor   = AppTheme.GridRow;
        dgv.DefaultCellStyle.ForeColor   = AppTheme.TextPrimary;
        dgv.DefaultCellStyle.SelectionBackColor = AppTheme.GridSelected;
        dgv.DefaultCellStyle.SelectionForeColor = Color.White;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridAlt;
        dgv.AlternatingRowsDefaultCellStyle.ForeColor = AppTheme.TextPrimary;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeader;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextSecondary;
        dgv.ColumnHeadersDefaultCellStyle.Font      = AppTheme.FontBodyBold;
        dgv.ColumnHeadersBorderStyle                = DataGridViewHeaderBorderStyle.None;
        dgv.EnableHeadersVisualStyles               = false;
        dgv.BorderStyle                             = BorderStyle.None;
        dgv.CellBorderStyle                         = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.RowHeadersVisible                       = false;
        dgv.SelectionMode                           = DataGridViewSelectionMode.FullRowSelect;
        dgv.Cursor                                  = Cursors.Default;
    }

    private static bool IsSubHeading(Label label) =>
        label.Font.Bold || label.Font.Size >= 11f;
}
