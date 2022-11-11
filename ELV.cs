using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace PCEnvanter
{
	public partial class ELV : ListView
	{
		ELVColumnHeaderCollection _columns;
		CellValueChangedEventArgs cea;
		RefreshItemDataEventArgs rea;

		public event ScrollEventHandler OnScroll;
		public event EventHandler<CellValueChangedEventArgs> OnCellValueChanged;
		public event EventHandler<RefreshItemDataEventArgs> OnCellDataRequested;

		private int SortColumn { get; set; } = -1;
		public List<string>? defData { get; set; }
		public List<ListViewItem>? Cache { get; set; } = new List<ListViewItem>();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ELVColumnHeaderCollection Columns { get { return _columns; } set { _columns = value; } }

		public ELV()
		{
			_columns = new ELVColumnHeaderCollection(this);

			InitializeComponent();
			this.OnScroll += EditableListView_OnScroll;
			this.ColumnClick += ELV_ColumnClick;
			Columns.OnHeadersAdded += Columns_OnHeadersAdded;
            
		}

        private void Columns_OnHeadersAdded(object? sender, EventArgs e)
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].EditControl == ControlTypes.ComboBox)
                {
                    (Columns[i].Editor as ComboBox).SelectionChangeCommitted += Combobox_SelectionChangeCommitted;
                    (Columns[i].Editor as ComboBox).KeyDown += Combobox_KeyDown;
                }
                else if (Columns[i].EditControl == ControlTypes.TextBox)
                {
                    (Columns[i].Editor as TextBox).KeyDown += textEditor_KeyDown;
                }
            }
        }

		private void ELV_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
			if (SortColumn == e.Column)
			{
				Cache?.Reverse();
			}
			else
			{
				if (Int32.TryParse(Cache?.First().SubItems[e.Column].Text, out _))
					Cache = Cache.OrderBy(x => Convert.ToInt32(x.SubItems[e.Column].Text)).ThenBy(x => Convert.ToInt32(x.SubItems[0].Text)).ToList();
				else
					Cache = Cache?.OrderBy(x => x.SubItems[e.Column].Text).ThenBy(x => Convert.ToInt32(x.SubItems[0].Text)).ToList();
			}

			SortColumn = e.Column;
			Refresh();
		}

        private void Combobox_SelectionChangeCommitted(object? sender, EventArgs e)
        {
            cea.NewValue = (string)((ComboBox)sender).SelectedValue;
            ((ListViewItem.ListViewSubItem)((ComboBox)sender).Tag).Text = cea.NewValue;

            if (this.OnCellValueChanged != null)
                OnCellValueChanged(this, cea);
        }


        private void EditableListView_OnScroll(object sender, ScrollEventArgs e)
		{
            //if (defEditor != null && defEditor.Visible)
            //    defEditor.Bounds = ((ListViewItem.ListViewSubItem)defEditor.Tag).Bounds;
            for (int i = 0; i < Columns.Count; i++)
            {
				if( Columns[i].Editor.Visible)
					Columns[i].Editor.Bounds = ((ListViewItem.ListViewSubItem)Columns[i].Editor.Tag).Bounds;
            }


        }

		private const int WM_HSCROLL = 0x114;
		private const int WM_VSCROLL = 0x115;
		private const int WM_MOUSEWHEEL = 0x020A;
		protected override void WndProc(ref Message msg)
		{
			base.WndProc(ref msg);
			if (msg.Msg == WM_MOUSEWHEEL || msg.Msg == WM_HSCROLL || msg.Msg == WM_VSCROLL)
			{
				if (this.OnScroll != null)
					OnScroll(this, new ScrollEventArgs((ScrollEventType)(msg.WParam.ToInt64() & 0xffff), 0));
			}
		}

		private void ELV_MouseUp(object sender, MouseEventArgs e)
		{
			Columns.DisableEditors();

			if (e.Button != MouseButtons.Left)
				return;


			ListViewItem lvItem = this.GetItemAt(e.X, e.Y);
			cea = new CellValueChangedEventArgs();
			rea = new RefreshItemDataEventArgs();

			if (lvItem == null)
				return;

			lvItem.Selected = true;

			Point mousePos = e.Location;
			ListViewHitTestInfo hitTest = HitTest(mousePos);
			if (hitTest.Item == null)
				return;

			int columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
			if (!Columns[columnIndex].Editable)
				return;

			cea.Column = Columns[columnIndex];
			cea.OldValue = hitTest.SubItem.Text;
			cea.Item = lvItem;

			if (hitTest.SubItem == null)
				return;

			if(Columns[columnIndex].EditControl == ControlTypes.ComboBox)
				(Columns[columnIndex].Editor as ComboBox).DataSource = defData;

			if (Columns[columnIndex].RequiresItemSpecificData)
			{
				rea.Column = Columns[columnIndex];
				rea.Item = lvItem;
				OnCellDataRequested(this, rea);
				(Columns[columnIndex].Editor as ComboBox).DataSource = (List<string>)Columns[columnIndex].Tag;
			}

			Columns[columnIndex].Editor!.Tag = hitTest.SubItem;
			Columns[columnIndex].Editor!.Bounds = hitTest.SubItem.Bounds;
			Columns[columnIndex].Editor!.Parent = this;
			Columns[columnIndex].Editor!.Text = hitTest.SubItem.Text;
			Columns[columnIndex].Editor!.Visible = true;
			Columns[columnIndex].Editor!.BringToFront();
			Columns[columnIndex].Editor!.Focus();

		}

		private void ELV_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				string defcol = "";
				foreach (ELVColumnHeader item in Columns)
				{
					if (item.DefaultEditColumn)
					{
						defcol = item.Text;
						break;
					}
				}

				if (defcol.Length < 1)
					return;

				if (SelectedIndices.Count < 1)
					return;

				Rectangle sb = Items[SelectedIndices[0]].SubItems[Columns[defcol].Index].Bounds;
				MouseEventArgs me = new MouseEventArgs(MouseButtons.Left, 1, sb.X + 1, sb.Y + 1, 1);
				ELV_MouseUp(null, me);
			}
		}
		private void textEditor_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			switch (e.KeyData)
			{
				case Keys.Escape:
					{
						textbox.Visible = false;
						break;
					}
				case Keys.Enter:
					{
						textbox.Visible = false;
						cea.NewValue = textbox.Text;
						((ListViewItem.ListViewSubItem)((TextBox)sender).Tag).Text = cea.NewValue;

						if (this.OnCellValueChanged != null)
							OnCellValueChanged(this, cea);
						break;
					}
				default:
					return;
			}
			this.Focus();
			e.SuppressKeyPress = true;
		}
		private void Combobox_KeyDown(object? sender, KeyEventArgs e)
		{
			ComboBox combobox = (sender as ComboBox);
			switch (e.KeyData)
			{
				case Keys.Escape:
					{
						combobox.Visible = false;
						break;
					}

				case Keys.Enter:
					{
						combobox.Visible = false;
						cea.NewValue = combobox.Text;
						((ListViewItem.ListViewSubItem)((ComboBox)sender).Tag).Text = cea.NewValue;

						if (this.OnCellValueChanged != null)
							OnCellValueChanged(this, cea);
						break;

					}
				default:
					return;
			}
			this.Focus();
			e.SuppressKeyPress = true;
		}
	}

	public static class ColumnHeaderCollectionExtension
	{
		public static bool editable = false;
		[Category("Behavior")]
		[Description("Allows cells to be edited")]
		public static bool IsEditable(this ColumnHeader self)
		{
			return editable;
		}
		public static void Editable(this ColumnHeader self, bool value)
		{
			editable = value;
		}
	}
	public class ELVColumnHeaderCollection : ColumnHeaderCollection
	{
		public event EventHandler<EventArgs> OnHeadersAdded;

		public new ELVColumnHeader this[string key]
		{
			get
			{
				return (ELVColumnHeader)base[key];
			}
		}
		public new ELVColumnHeader this[int index]
		{
			get
			{
				return (ELVColumnHeader)base[index];
			}
		}

		public new void AddRange(ColumnHeader[] values)
        {
			base.AddRange(values);
			OnHeadersAdded(this, new EventArgs());
        }
		public void DisableEditors()
        {
            for (int i = 0; i < Count; i++)
            {
				this[i].Editor.Hide();
            }
        }

		public ELVColumnHeaderCollection(ListView _owner) : base(_owner)
		{
		}
	}
	public class ELVListViewItemCollection : ListViewItemCollection
	{
		List<ListViewItem> items = new List<ListViewItem>();
		ListView owner;

		public ELVListViewItemCollection(ListView _owner) : base(_owner)
		{
			owner = _owner;
		}

		public override ListViewItem? this[int index]
		{
			get
			{
				foreach (var item in items)
				{
					if (item.Index == index)
						return item;
				}
				return null;
			}
			set
			{
				items[index] = value;
			}
		}

		public new ListViewItem this[string column]
		{
			get
			{
				return this[owner.Columns.IndexOfKey(column)];
			}
			set
			{
				this[owner.Columns.IndexOfKey(column)] = value;
			}
		}
	}

	public enum ControlTypes { TextBox, ComboBox, CheckBox };

	public class ELVColumnHeader : ColumnHeader
	{
		private ControlTypes ct = ControlTypes.TextBox;
		public Control? Editor { get; set; } = new TextBox();

		[Category("Behavior")]
		[Description("Allows cells to be edited")]
		public bool Editable { get; set; }

		[Category("Behavior")]
		[Description("Type of Editing Control")]
		public ControlTypes EditControl { 
			get 
			{ 
				return ct;
			}
			set 
			{
				ct = value;
				if (value == ControlTypes.ComboBox)
					Editor = new ComboBox();
				else if (value == ControlTypes.CheckBox)
					Editor = new CheckBox();
			}
		}

		[Category("Behavior")]
		[Description("Is this column act as default edit column or not?")]
		public bool DefaultEditColumn { get; set; } = false;

		[Category("Behavior")]
		[Description("Is this column requires data that specific to item selected?")]
		public bool RequiresItemSpecificData { get; set; } = false;

		public new string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (Name == null || Name.Length < 1)
					Name = value;
				base.Text = value;
			}
		}
	}

	public class RefreshItemDataEventArgs : EventArgs
	{
		public ListViewItem? Item { get; set; }

		public ELVColumnHeader? Column { get; set; }
	}

	public class CellValueChangedEventArgs : EventArgs
	{
		public string? NewValue { get; set; }
		public string? OldValue { get; set; }
		public ListViewItem? Item { get; set; }

		public ELVColumnHeader? Column { get; set; }
		//public string ColumnName { get; set; }
		//public int ColumnOrder { get; set; }
	}

	public class ListViewColumnSorter : IComparer
	{
		/// <summary>
		/// Specifies the column to be sorted
		/// </summary>
		private int ColumnToSort;

		/// <summary>
		/// Specifies the order in which to sort (i.e. 'Ascending').
		/// </summary>
		private SortOrder OrderOfSort;

		/// <summary>
		/// Case insensitive comparer object
		/// </summary>
		private CaseInsensitiveComparer ObjectCompare;

		/// <summary>
		/// Class constructor. Initializes various elements
		/// </summary>
		public ListViewColumnSorter()
		{
			// Initialize the column to '0'
			ColumnToSort = 0;

			// Initialize the sort order to 'none'
			OrderOfSort = SortOrder.None;

			// Initialize the CaseInsensitiveComparer object
			ObjectCompare = new CaseInsensitiveComparer();
		}

		/// <summary>
		/// This method is inherited from the IComparer interface. It compares the two objects passed using a case insensitive comparison.
		/// </summary>
		/// <param name="x">First object to be compared</param>
		/// <param name="y">Second object to be compared</param>
		/// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
		public int Compare(object x, object y)
		{
			int compareResult;
			ListViewItem listviewX, listviewY;

			// Cast the objects to be compared to ListViewItem objects
			listviewX = (ListViewItem)x;
			listviewY = (ListViewItem)y;

			// Compare the two items
			int a = 0;
			int b = 0;

			if (Int32.TryParse(listviewX.SubItems[ColumnToSort].Text, out a) && Int32.TryParse(listviewY.SubItems[ColumnToSort].Text, out b))
			{
				compareResult = ObjectCompare.Compare(a, b);
			}
			else
			{
				compareResult = ObjectCompare.Compare(listviewY.SubItems[ColumnToSort].Text, listviewX.SubItems[ColumnToSort].Text );
			}


			if (compareResult == 0)
			{
				int index = listviewX.ListView.Columns["Order"].Index;

				compareResult = ObjectCompare.Compare(Convert.ToInt32(listviewY.SubItems[index].Text), Convert.ToInt32(listviewX.SubItems[index].Text));
				return -compareResult;
			}

			// Calculate correct return value based on object comparison
			else if (OrderOfSort == SortOrder.Ascending)
			{
				// Ascending sort is selected, return normal result of compare operation
				return compareResult;
			}
			else if (OrderOfSort == SortOrder.Descending)
			{
				// Descending sort is selected, return negative result of compare operation
				return (-compareResult);
			}
			else
			{
				// Return '0' to indicate they are equal
				return 0;
			}
		}

		/// <summary>
		/// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
		/// </summary>
		public int SortColumn
		{
			set
			{
				ColumnToSort = value;
			}
			get
			{
				return ColumnToSort;
			}
		}

		/// <summary>
		/// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
		/// </summary>
		public SortOrder Order
		{
			set
			{
				OrderOfSort = value;
			}
			get
			{
				return OrderOfSort;
			}
		}

	}

}
