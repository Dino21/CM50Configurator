using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CM50_Configurator.Support.UI
{
    // Tema visivo Sesami applicato a tutte le finestre dell'app: colori, font di brand
    // (Neue Haas Unica Pro, incorporato come risorsa) e stile dei controlli standard WinForms.
    // Punto d'ingresso unico: SesamiTheme.Apply(this) chiamato da ogni Form dopo InitializeComponent().
    public static class SesamiTheme
    {
        // Palette dalla brand guide Sesami (sezione 3.1/3.2 "Color") - nessun colore inventato.
        public static readonly Color Background = Color.FromArgb(0x25, 0x25, 0x2D);      // Dark Gray / Gray 2
        public static readonly Color BackgroundDeep = Color.FromArgb(0x19, 0x19, 0x1F);   // Gray 1
        public static readonly Color Surface = Color.FromArgb(0x36, 0x36, 0x3E);          // Gray 3
        public static readonly Color Border = Color.FromArgb(0x3F, 0x3F, 0x46);           // Gray 4
        public static readonly Color SurfaceHover = Color.FromArgb(0x3F, 0x3F, 0x46);     // Gray 4
        public static readonly Color TextPrimary = Color.FromArgb(0xF6, 0xF6, 0xF7);      // Gray 10
        public static readonly Color TextSecondary = Color.FromArgb(0x92, 0x92, 0x96);    // Gray 7
        public static readonly Color Accent = Color.FromArgb(0x5C, 0xE5, 0xDB);           // Aqua
        public static readonly Color AccentHover = Color.FromArgb(0x00, 0xA6, 0x99);       // Aqua 1
        private static readonly Color Yellow1 = Color.FromArgb(0xE7, 0xF2, 0x18);          // Yellow 1
        public static readonly Color OnAccent = Background;                                // testo scuro su Accent

        // Righe di diff nelle finestre di revisione modifiche: stesse tinte semantiche originali
        // (verde = nuovo, giallo = modificato) ma calcolate come sfumature scure leggibili sul tema.
        public static readonly Color NewRowColor = Blend(Background, AccentHover, 0.25f);
        public static readonly Color ModifiedRowColor = Blend(Background, Yellow1, 0.25f);
        private static readonly Color AccentPressed = Blend(AccentHover, Background, 0.35f);
        private static readonly Color SurfacePressed = Blend(Border, Background, 0.35f);

        private static readonly PrivateFontCollection FontCollection = new PrivateFontCollection();
        private static FontFamily _brandRegular;
        private static FontFamily _brandBold;
        private static bool _fontsLoaded;

        private static Color Blend(Color from, Color to, float t)
        {
            int r = (int)Math.Round(from.R + (to.R - from.R) * t);
            int g = (int)Math.Round(from.G + (to.G - from.G) * t);
            int b = (int)Math.Round(from.B + (to.B - from.B) * t);
            return Color.FromArgb(r, g, b);
        }

        private static void EnsureFontsLoaded()
        {
            if (_fontsLoaded)
            {
                return;
            }

            _fontsLoaded = true;

            try
            {
                LoadEmbeddedFont("CM50_Configurator.Assets.NeueHaasUnicaPro-Regular.ttf");
                LoadEmbeddedFont("CM50_Configurator.Assets.NeueHaasUnicaPro-Bold.ttf");

                foreach (FontFamily family in FontCollection.Families)
                {
                    if (family.Name.IndexOf("Bold", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _brandBold = family;
                    }
                    else
                    {
                        _brandRegular = family;
                    }
                }

                if (_brandBold == null)
                {
                    _brandBold = _brandRegular;
                }
            }
            catch
            {
                // Se il font di brand non è disponibile per qualsiasi motivo, si resta sui
                // font di sistema: l'app deve continuare a funzionare comunque.
                _brandRegular = null;
                _brandBold = null;
            }
        }

        private static void LoadEmbeddedFont(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return;
                }

                byte[] fontData = new byte[stream.Length];
                stream.Read(fontData, 0, fontData.Length);

                IntPtr fontPtr = Marshal.AllocHGlobal(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                // Il buffer non va liberato: PrivateFontCollection lo referenzia per tutta la
                // durata del processo. Viene rilasciato dal sistema alla chiusura dell'app.
                FontCollection.AddMemoryFont(fontPtr, fontData.Length);
            }
        }

        // Crea un font di brand mantenendo dimensione/stile del font originale del controllo,
        // per non alterare i layout esistenti (basati su AutoScaleMode.Font / AutoSize).
        public static Font GetFont(float size, FontStyle style)
        {
            EnsureFontsLoaded();

            FontFamily family = (style & FontStyle.Bold) == FontStyle.Bold ? _brandBold : _brandRegular;
            if (family == null)
            {
                return new Font("Segoe UI", size, style);
            }

            try
            {
                return new Font(family, size, style);
            }
            catch
            {
                return new Font("Segoe UI", size, style);
            }
        }

        private static Icon _appIcon;
        private static bool _appIconLoaded;

        public static Icon AppIcon
        {
            get
            {
                if (!_appIconLoaded)
                {
                    _appIconLoaded = true;
                    try
                    {
                        _appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                    }
                    catch
                    {
                        _appIcon = null;
                    }
                }

                return _appIcon;
            }
        }

        private static Image _logoImage;
        private static bool _logoLoaded;

        private static Image LogoImage
        {
            get
            {
                if (!_logoLoaded)
                {
                    _logoLoaded = true;
                    try
                    {
                        Assembly asm = Assembly.GetExecutingAssembly();
                        using (Stream stream = asm.GetManifestResourceStream("CM50_Configurator.Assets.sesami-logo-2c-rgb-neg.png"))
                        {
                            _logoImage = stream != null ? Image.FromStream(stream) : null;
                        }
                    }
                    catch
                    {
                        _logoImage = null;
                    }
                }

                return _logoImage;
            }
        }

        public const int HeaderBannerHeight = 64;

        public static void Apply(Form form)
        {
            EnsureFontsLoaded();

            // Il Font va cambiato DOPO che i controlli sono già stati creati e posizionati (qui,
            // o nel caso dei form costruiti a mano come SimpleEntityDetailForm/BulkCodeReplaceForm,
            // subito dopo BuildUi()). Con AutoScaleMode.Font ancora attivo, quel cambio di Font
            // innesca un riscalo automatico delle posizioni/anchor dei controlli rispetto alle
            // metriche del nuovo font: per i controlli ancorati a destra/basso il risultato è
            // inconsistente e può spingerli fuori dall'area visibile del form (bottone "Chiudi"
            // introvabile nei dettagli di ddmcm50/utmcm50/ltmcm50/feedercm50). I layout dell'app
            // usano coordinate fisse in pixel e non contano su un riscalo automatico, quindi va
            // disattivato prima di applicare il font di brand.
            form.AutoScaleMode = AutoScaleMode.None;

            form.BackColor = Background;
            form.ForeColor = TextPrimary;
            form.Font = GetFont(form.Font.Size, form.Font.Style);

            if (AppIcon != null)
            {
                form.Icon = AppIcon;
            }

            var acceptButton = form.AcceptButton as Button;
            StyleContainer(form, acceptButton);
        }

        private static void StyleContainer(Control root, Button primaryButton)
        {
            foreach (Control control in root.Controls)
            {
                StyleControl(control, primaryButton);

                // La ricorsione si ferma ai contenitori generici usati nel codebase (Panel,
                // GroupBox): scendere anche dentro DataGridView/PropertyGrid/ComboBox
                // rischierebbe di ri-stilare i loro controlli interni (editor di cella,
                // scrollbar, popup) e rompere il rendering nativo di quei controlli.
                if (control is Panel || control is GroupBox)
                {
                    StyleContainer(control, primaryButton);
                }
            }
        }

        private static void StyleControl(Control control, Button primaryButton)
        {
            switch (control)
            {
                case Button button:
                    StyleButton(button, button == primaryButton || button.Font.Bold);
                    break;

                case Label label:
                    label.ForeColor = TextPrimary;
                    break;

                case GroupBox groupBox:
                    groupBox.ForeColor = TextPrimary;
                    break;

                case CheckBox checkBox:
                    checkBox.ForeColor = TextPrimary;
                    checkBox.Font = GetFont(checkBox.Font.Size, checkBox.Font.Style);
                    break;

                case TextBox textBox:
                    textBox.BackColor = Surface;
                    textBox.ForeColor = TextPrimary;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    textBox.Font = GetFont(textBox.Font.Size, textBox.Font.Style);
                    break;

                case ComboBox comboBox:
                    comboBox.FlatStyle = FlatStyle.Flat;
                    comboBox.BackColor = Surface;
                    comboBox.ForeColor = TextPrimary;
                    comboBox.Font = GetFont(comboBox.Font.Size, comboBox.Font.Style);
                    break;

                case ListBox listBox:
                    StyleListBox(listBox);
                    break;

                case DataGridView grid:
                    StyleDataGridView(grid);
                    break;

                case PropertyGrid propertyGrid:
                    StylePropertyGrid(propertyGrid);
                    break;

                case Panel panel:
                    StylePanel(panel);
                    break;
            }
        }

        private static void StyleButton(Button button, bool primary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.Cursor = Cursors.Hand;

            if (primary)
            {
                button.Font = GetFont(button.Font.Size, FontStyle.Bold);
                button.BackColor = Accent;
                button.ForeColor = OnAccent;
                button.FlatAppearance.BorderColor = Accent;
                button.FlatAppearance.MouseOverBackColor = AccentHover;
                button.FlatAppearance.MouseDownBackColor = AccentPressed;
            }
            else
            {
                button.Font = GetFont(button.Font.Size, button.Font.Style);
                button.BackColor = Surface;
                button.ForeColor = TextPrimary;
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.MouseOverBackColor = SurfaceHover;
                button.FlatAppearance.MouseDownBackColor = SurfacePressed;
            }
        }

        private static void StylePanel(Panel panel)
        {
            bool isActionBar = panel.Parent is Form && (panel.Dock == DockStyle.Top || panel.Dock == DockStyle.Bottom);

            if (isActionBar)
            {
                panel.BackColor = BackgroundDeep;

                panel.Paint -= ActionBarPanel_Paint;
                panel.Paint += ActionBarPanel_Paint;
            }
            else
            {
                panel.BackColor = Background;
            }
        }

        private static void ActionBarPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            using (var pen = new Pen(Border))
            {
                if (panel.Dock == DockStyle.Bottom)
                {
                    e.Graphics.DrawLine(pen, 0, 0, panel.Width, 0);
                }
                else if (panel.Dock == DockStyle.Top)
                {
                    e.Graphics.DrawLine(pen, 0, panel.Height - 1, panel.Width, panel.Height - 1);
                }
            }
        }

        private static void StyleListBox(ListBox listBox)
        {
            listBox.BackColor = Surface;
            listBox.ForeColor = TextPrimary;
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.Font = GetFont(listBox.Font.Size, listBox.Font.Style);
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.ItemHeight = Math.Max(listBox.ItemHeight, 20);

            listBox.DrawItem -= ListBox_DrawItem;
            listBox.DrawItem += ListBox_DrawItem;
        }

        private static void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            var listBox = (ListBox)sender;
            if (e.Index < 0 || e.Index >= listBox.Items.Count)
            {
                e.DrawBackground();
                return;
            }

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color back = selected ? Accent : Surface;
            Color fore = selected ? OnAccent : TextPrimary;

            using (var backBrush = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            string text = listBox.GetItemText(listBox.Items[e.Index]);
            using (var foreBrush = new SolidBrush(fore))
            {
                var rect = new RectangleF(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 12, e.Bounds.Height);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(text, listBox.Font, foreBrush, rect, format);
            }
        }

        private static void StyleDataGridView(DataGridView grid)
        {
            grid.BackgroundColor = Background;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersDefaultCellStyle.BackColor = BackgroundDeep;
            grid.RowHeadersDefaultCellStyle.ForeColor = TextSecondary;
            grid.RowHeadersDefaultCellStyle.SelectionBackColor = BackgroundDeep;
            grid.RowHeadersDefaultCellStyle.SelectionForeColor = Accent;

            grid.ColumnHeadersDefaultCellStyle.BackColor = BackgroundDeep;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = BackgroundDeep;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.Font = GetFont(grid.Font.Size, FontStyle.Bold);
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = Accent;
            grid.DefaultCellStyle.SelectionForeColor = OnAccent;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Background;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Accent;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = OnAccent;

            grid.Font = GetFont(grid.Font.Size, grid.Font.Style);
        }

        private static void StylePropertyGrid(PropertyGrid grid)
        {
            grid.BackColor = Background;
            grid.ViewBackColor = Surface;
            grid.ViewForeColor = TextPrimary;
            grid.LineColor = Border;
            grid.CategorySplitterColor = BackgroundDeep;
            grid.CategoryForeColor = Accent;
            grid.HelpBackColor = BackgroundDeep;
            grid.HelpForeColor = TextSecondary;
            grid.CommandsBackColor = BackgroundDeep;
            grid.CommandsForeColor = TextPrimary;
            grid.DisabledItemForeColor = TextSecondary;
            grid.Font = GetFont(grid.Font.Size, grid.Font.Style);
        }

        // Valore sentinella usato dal bottone "Menu principale" per segnalare a Program.cs che,
        // alla chiusura di questo form, va ri-mostrato lo StartForm invece di terminare l'app.
        public const DialogResult BackToMenuResult = DialogResult.Retry;

        // Fascia superiore con il logo Sesami, usata solo su StartForm e sulle schermate elenco
        // principali (una per modulo). Va aggiunta ai Controls del form DOPO gli altri controlli,
        // stesso ordine già usato con successo nel codebase per i pannelli Dock=Bottom, cosicché
        // il contenuto Dock=Fill si restringe automaticamente per fare spazio al banner.
        // showBackToMenu aggiunge, ancorato a destra, un bottone per tornare allo StartForm senza
        // chiudere l'applicazione (non usato sullo StartForm stesso, dove non avrebbe senso).
        public static void AddHeaderBanner(Form form, string subtitle, bool showBackToMenu = true)
        {
            var banner = new Panel
            {
                Dock = DockStyle.Top,
                Height = HeaderBannerHeight,
                // Larghezza impostata esplicitamente da subito (uguale al form): il pannello non è
                // ancora aggiunto a form.Controls a questo punto, quindi Dock=Top non gli ha ancora
                // dato la sua larghezza reale. Senza questo, il bottone "Menu principale" (Anchor
                // Right, aggiunto sotto) calcolerebbe la sua distanza dal bordo destro rispetto alla
                // larghezza di default del Panel (200px) invece che a quella vera del form, e una
                // volta che il banner viene agganciato finirebbe fuori dall'area visibile.
                Width = form.ClientSize.Width,
                BackColor = BackgroundDeep
            };
            banner.Paint += ActionBarPanel_Paint;

            Image logo = LogoImage;
            if (logo != null)
            {
                double aspect = (double)logo.Width / logo.Height;
                int logoHeight = 28;
                int logoWidth = (int)Math.Round(logoHeight * aspect);

                var pictureBox = new PictureBox
                {
                    Image = logo,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(logoWidth, logoHeight),
                    Location = new Point(20, (HeaderBannerHeight - logoHeight) / 2),
                    BackColor = Color.Transparent
                };
                banner.Controls.Add(pictureBox);

                if (!string.IsNullOrEmpty(subtitle))
                {
                    var divider = new Label
                    {
                        Text = string.Empty,
                        BackColor = Border,
                        Size = new Size(1, 28),
                        Location = new Point(pictureBox.Right + 16, (HeaderBannerHeight - 28) / 2)
                    };
                    banner.Controls.Add(divider);

                    var subtitleLabel = new Label
                    {
                        Text = subtitle,
                        AutoSize = true,
                        ForeColor = TextPrimary,
                        Font = GetFont(11.25F, FontStyle.Bold),
                        BackColor = Color.Transparent
                    };
                    banner.Controls.Add(subtitleLabel);
                    subtitleLabel.Location = new Point(divider.Right + 16, (HeaderBannerHeight - subtitleLabel.PreferredHeight) / 2);
                }
            }
            else if (!string.IsNullOrEmpty(subtitle))
            {
                var subtitleLabel = new Label
                {
                    Text = subtitle,
                    AutoSize = true,
                    ForeColor = TextPrimary,
                    Font = GetFont(11.25F, FontStyle.Bold),
                    BackColor = Color.Transparent,
                    Location = new Point(20, 0)
                };
                banner.Controls.Add(subtitleLabel);
                subtitleLabel.Location = new Point(20, (HeaderBannerHeight - subtitleLabel.PreferredHeight) / 2);
            }

            if (showBackToMenu)
            {
                var btnBack = new Button
                {
                    Text = "‹ Menu principale",
                    Font = GetFont(9F, FontStyle.Regular),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Padding = new Padding(10, 0, 10, 0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                StyleButton(btnBack, primary: false);

                Size preferredSize = btnBack.GetPreferredSize(Size.Empty);
                btnBack.Height = Math.Max(preferredSize.Height, 27);
                btnBack.Width = preferredSize.Width;
                btnBack.Location = new Point(
                    form.ClientSize.Width - btnBack.Width - 20,
                    (HeaderBannerHeight - btnBack.Height) / 2);

                btnBack.Click += (s, e) =>
                {
                    form.DialogResult = BackToMenuResult;
                    form.Close();
                };

                banner.Controls.Add(btnBack);
            }

            form.Controls.Add(banner);
        }
    }
}
