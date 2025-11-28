
namespace kms.View
{
    using kms.Models;
    using kms.Util;
    using log4net;
    using Terminal.Gui;

    /// <summary>
    /// Main CLI menu for navigating kennel management operations.
    /// </summary>
    public abstract class AbstractWindow : Window
    {

        protected readonly BaseNavigation _navigation;

        protected abstract ILog _log { get; }

        private FrameView? _detailsFrame = null;
        private FrameView? _resultsFrame = null;

        public AbstractWindow(BaseNavigation navigation, string title, bool withBorder = true)
        {
            _log.Info($"Opening new window: {title}");
            _navigation = navigation;
            InitializeWindow(title, withBorder);
        }

        private void InitializeWindow(string title, bool withBorder)
        {
            // dimentions
            Width = Dim.Fill(0);
            Height = Dim.Fill(0);
            X = 0;
            Y = 0;

            // styling
            Border.BorderStyle = BorderStyle.None;
            Border.Effect3D = false;
            Border.DrawMarginFrame = false;
            Modal = false;
            TextAlignment = TextAlignment.Left;

            // customisation
            if (withBorder)
            {
                Y = 1;
                Title = StringUtils.GetTitle(title);
                Border.BorderStyle = BorderStyle.Single;
                Border.Effect3D = true;
                Border.DrawMarginFrame = true;
            }
        }

        protected abstract void InitializeWindowContent();

        protected FrameView BuildDetailsFrame(string frameLabel, Pos col, Pos row, Dim? height = null, Dim? width = null)
        {
            _detailsFrame = BuildFrame(_detailsFrame, frameLabel, col, row, height, width);
            return _detailsFrame;
        }

        protected FrameView BuildResultsFrame(string frameLabel, Pos col, Pos row, Dim? height = null, Dim? width = null)
        {
            _resultsFrame = BuildFrame(_resultsFrame, frameLabel, col, row, height, width);
            return _resultsFrame;
        }

        private FrameView BuildFrame(FrameView? frame, string frameLabel, Pos col, Pos row, Dim? height = null, Dim? width = null)
        {
            if (frame != null)
            {
                frame.Dispose();
                Remove(frame);
            }
            frame = ViewUtils.CreateFrame(this, frameLabel, col, row, height ?? (Dim.Fill() - 2), width);
            return frame;
        }

        protected bool NoResultsFound<T>(FrameView frame, List<T> results, string? message = null)
        {
            if (results.Count == 0)
            {
                ViewUtils.CreateLabel(frame, message ?? "No results found!", Pos.Center(), Pos.Center());
                return true;
            }
            return false;
        }

        protected TableView? ShowKennelsTable(FrameView parent, List<Kennel> results, Action<FrameView, Button?, int, Kennel>? selectionAction = null, bool simple = true)
        {
            if (!NoResultsFound<Kennel>(parent, results, "No kennels available for the selected period!"))
            {
                List<Dictionary<string, object?>> values = [];
                foreach (Kennel kennel in results)
                {
                    Dictionary<string, object?> entry = [];
                    entry["ID"] = kennel.Id;
                    entry["Name"] = kennel.Name;
                    entry["Size"] = $"{kennel.Size}";
                    if (!simple)
                    {
                        entry["Suitable For"] = $"{kennel.SuitableFor}";
                        entry["Out-of-Service"] = kennel.IsOutOfService;
                        entry["Reason"] = kennel.OutOfServiceComment ?? "";
                    }
                    values.Add(entry);
                }

                TableView kennelTable = ViewUtils.CreateTable(parent, 1, 1, Dim.Fill() - 2, Dim.Fill() - 1, values);
                Button? action = null;
                kennelTable.SelectedCellChanged += (args) =>
                {
                    int id = (int)args.Table.Rows[kennelTable.SelectedRow][0];
                    selectionAction?.Invoke(parent, action, id, results.First(o => o.Id == id));
                };
                return kennelTable;
            }
            return null;
        }

        protected void ResetWindow(ApplicationWindowsEnum? returnWindow = null)
        {
            // cleanup UI
            _detailsFrame?.Dispose();
            _resultsFrame?.Dispose();
            RemoveAll();
            // Add return button (if required)
            if (returnWindow != null)
            {
                Button returnBtn = ViewUtils.CreateButton(this, "Return", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
                returnBtn.Clicked += () => _navigation.GotoWindow(returnWindow);
            }
        }

    }

    /// <summary>
    /// Main CLI menu for navigating kennel management operations.
    /// </summary>
    public abstract class AbstractMenu : AbstractWindow
    {

        protected abstract Dictionary<string, ApplicationWindowsEnum?> _items { get; }

        public AbstractMenu(BaseNavigation navigation, string title) : base(navigation, title)
        {
            InitializeWindowContent();
        }

        protected override void InitializeWindowContent()
        {
            Add(new Label("Please select from of the following options: -\n(enter the menu number / use the arrow keys & press enter / or use your mouse)")
            {
                X = 2,
                Y = 1,
                Width = Dim.Fill() - 2
            });

            ListView listView = new(_items.Keys.ToList())
            {
                X = 2,
                Y = 4,
                Height = Dim.Sized(_items.Count),
                Width = Dim.Fill() - 2
            };
            Add(listView);

            // capture selected menu item navigation view keyboard
            bool triggerAction = false;
            listView.KeyPress += (args) =>
            {
                int keyValue = args.KeyEvent.KeyValue;
                // user pressed enter to select a menu
                if (keyValue == 10)
                {
                    args.Handled = true;
                    GotoSelectedView(listView.SelectedItem);
                }
                // use entered a numbered to select a menu
                else if (keyValue >= 48 && keyValue < 48 + _items.Count)
                {
                    triggerAction = true;
                }
            };

            // track mouse action and redirect to selected menu
            listView.MouseClick += (args) => triggerAction = true;
            listView.SelectedItemChanged += (args) =>
            {
                if (triggerAction)
                {
                    triggerAction = false;
                    GotoSelectedView(listView.SelectedItem);
                }
            };
        }

        protected abstract void GotoSelectedView(int selectedItem);

    }

}
