
namespace kms.Util
{

    using System.Data;

    using NStack;
    using Terminal.Gui;

    /// <summary>
    /// View utilities.
    /// </summary>
    public static class ViewUtils
    {

        private static readonly int COL = 2;
        private static readonly Dim WIDTH = Dim.Fill() - 2;

        public static Button CreateButton(View parent, string buttonLabel, Pos col, Pos row, ColorScheme? colorScheme = null)
        {
            Button button = new(buttonLabel)
            {
                X = col,
                Y = row,
                ColorScheme = colorScheme ?? Colors.Base
            };
            parent.Add(button);
            return button;
        }

        public static DateField CreateDateField(View parent, string fieldLabel, Pos row, DateTime? value = null, bool enabled = true) =>
            CreateDateField(parent, fieldLabel, COL, row, WIDTH, value, enabled);


        public static DateField CreateDateField(View parent, string fieldLabel, Pos col, Pos row, Dim width, DateTime? value = null, bool enabled = true)
        {
            Label label = CreateLabel(parent, fieldLabel, col, row);
            DateField field = new(value ?? DateTime.Today)
            {
                X = Pos.Right(label) + 1,
                Y = row,
                Width = width,
                Enabled = enabled
            };
            parent.Add(field);
            return field;
        }

        public static FrameView CreateFrame(View parent, string frameLabel, Pos col, Pos row, Dim height, Dim? width = null)
        {
            FrameView frame = new(frameLabel)
            {
                X = col,
                Y = row,
                Height = height,
                Width = width ?? WIDTH
            };
            parent.Add(frame);
            return frame;
        }

        public static Label CreateLabel(View parent, string labelText, Pos row, ColorScheme? colorScheme = null) =>
            CreateLabel(parent,labelText, COL, row, colorScheme);

        public static Label CreateLabel(View parent, string labelText, Pos col, Pos row, ColorScheme? colorScheme = null) =>
            CreateLabel(parent,labelText, col, row, null, null, colorScheme);

        public static Label CreateLabel(View parent, string labelText, Pos col, Pos row, Dim? height, Dim? width, ColorScheme? colorScheme = null)
        {
            Label label = new(labelText)
            {
                X = col,
                Y = row,
                Height = height,
                Width = width,
                ColorScheme = colorScheme ?? Colors.Base
            };
            parent.Add(label);
            return label;
        }

        public static RadioGroup CreateRadioGroup<TEnum>(View parent, string fieldLabel, Pos row, string? value = null, bool enabled = true) where TEnum : struct, Enum =>
            CreateRadioGroup<TEnum>(parent, fieldLabel, COL, row,  value, enabled);

        public static RadioGroup CreateRadioGroup<TEnum>(View parent, string fieldLabel, Pos col, Pos row, string? value = null, bool enabled = true) where TEnum : struct, Enum
        {
            Label label = CreateLabel(parent, fieldLabel, col, row);
            return CreateRadioGroup<TEnum>(parent, label, value, enabled);
        }

        public static RadioGroup CreateRadioGroup<TEnum>(View parent, Label label, string? value = null, bool enabled = true) where TEnum : struct, Enum
        {
            ustring[] items = EnumUtils.GetNamesList(typeof(TEnum));
            return CreateRadioGroup(parent, label, items, value, enabled);
        }

        public static RadioGroup CreateRadioGroup(View parent, string fieldLabel, Pos row, ustring[] items, string? value = null, bool enabled = true) =>
            CreateRadioGroup(parent, fieldLabel, COL, row, items, value, enabled);

        public static RadioGroup CreateRadioGroup(View parent, string fieldLabel, Pos col, Pos row, ustring[] items, string? value = null, bool enabled = true)
        {
            Label label = CreateLabel(parent, fieldLabel, col, row);
            return CreateRadioGroup(parent,label,items, value,enabled);
        }

        private static RadioGroup CreateRadioGroup(View parent, Label label, ustring[] items, string? value = null, bool enabled = true)
        {
            RadioGroup field = new(items)
            {
                X = Pos.Right(label) + 1,
                Y = label.Y,
                DisplayMode = DisplayModeLayout.Horizontal,
                Enabled = enabled,
                // SelectedItem = -1
            };
            if (value != null)
            {
                field.SelectedItem = Array.FindIndex(items, i => i.Equals(value));
            }
            parent.Add(field);
            return field;
        }

        public static TextField CreateTextField(View parent, string fieldLabel, Pos row, string? value = null, bool enabled = true)    {
            Label label = CreateLabel(parent, fieldLabel, row);
            return  CreateTextField(parent, label, WIDTH, value, enabled);
        }

        public static TextField CreateTextField(View parent, Label label, string? value = null, bool enabled = true) =>
            CreateTextField(parent, label, WIDTH, value, enabled);

        public static TextField CreateTextField(View parent, string fieldLabel, Pos col, Pos row, Dim width, string? value = null, bool enabled = true)
        {
            Label label = CreateLabel(parent, fieldLabel, col, row);
            return CreateTextField(parent, label, width, value, enabled);
        }

        public static TextField CreateTextField(View parent, Label label, Dim width, string? value = null, bool enabled = true)
        {
            TextField field = new()
            {
                X = Pos.Right(label) + 1,
                Y = label.Y,
                Width = width,
                Enabled = enabled,
                Text = value ?? ""
            };
            parent.Add(field);
            return field;
        }

        public static TableView CreateTable(
            View parent, Pos col, Pos row, Dim height, Dim width,
            List<Dictionary<string, object?>> values
        )
        {
            // define the table columns
            DataTable dataTable = new();
            foreach (var item in values[0])
            {
                dataTable.Columns.Add(new DataColumn(item.Key, item.Value?.GetType() ?? typeof(string)));
            }
            // define the table rows
            List<CheckBox> tableSelect = [];
            foreach (var entry in values)
            {
                dataTable.Rows.Add([.. entry.Values]);
            }
            // build the results view
            TableView table = new(dataTable)
            {
                X = col,
                Y = row,
                Height = height,
                Width = width,
                FullRowSelect = true
            };
            table.Style.AlwaysShowHeaders = true;
            parent.Add(table);
            return table;
        }

        public static int DisplayError(string title, string message, List<string>? actions = null)
        {
            actions ??= ["_OK"];
            return MessageBox.ErrorQuery(title, $"\n{message}", [.. actions.Select(action => ustring.Make(action))]);
        }

        public static int DisplayMessage(string title, string message, List<string>? actions = null)
        {
            actions ??= ["_OK"];
            return MessageBox.Query(title, $"\n{message}", 0, [.. actions.Select(action => ustring.Make(action))]);
        }

    }

}