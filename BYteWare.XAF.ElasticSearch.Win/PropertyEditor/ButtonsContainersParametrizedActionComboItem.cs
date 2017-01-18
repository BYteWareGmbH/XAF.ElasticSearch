namespace BYteWare.XAF.ElasticSearch.Win.PropertyEditor
{
    using DevExpress.ExpressApp.Actions;
    using DevExpress.ExpressApp.Templates;
    using DevExpress.ExpressApp.Utils;
    using DevExpress.ExpressApp.Win.Templates.ActionContainers;
    using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
    using DevExpress.XtraEditors;
    using DevExpress.XtraEditors.Controls;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// Action Item to enable the display of suggestions and start the search per button or Enter
    /// </summary>
    [CLSCompliant(false)]
    public class ButtonsContainersParametrizedActionComboItem : ButtonsContainersParametrizedActionItem
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="ButtonsContainersParametrizedActionComboItem"/> class.
        /// </summary>
        /// <param name="action">Parametrized Action instance</param>
        /// <param name="owner">Buttons Container</param>
        public ButtonsContainersParametrizedActionComboItem(ParametrizedAction action, ButtonsContainer owner)
            : base(action, owner)
        {
        }

        private EditorButton goButton;

        /// <inheritdoc/>
        protected override Control CreateControl()
        {
            ButtonEdit tempControl = null;
            ButtonEdit control = null;
            try
            {
                var type = GetType().BaseType;
#pragma warning disable CC0009 // Use object initializer
                tempControl = new ComboBoxEdit();
#pragma warning restore CC0009 // Use object initializer
                tempControl.AutoSizeInLayoutControl = false;
                tempControl.MinimumSize = new Size(150, 20);
                tempControl.Properties.NullValuePrompt = Action.NullValuePrompt;
                tempControl.Properties.NullValuePromptShowForEmptyValue = true;
                if (ShowExecuteButton)
                {
                    goButton = new EditorButton
                    {
                        Kind = ButtonPredefines.Glyph,
                        Tag = ParametrizedActionItemControlFactory.GoButtonID,
                        ImageLocation = ImageLocation.TopLeft
                    };
                    var imageInfo = default(ImageInfo);
                    if (!string.IsNullOrEmpty(Action.ImageName))
                    {
                        imageInfo = ImageLoader.Instance.GetImageInfo(Action.ImageName);
                    }
                    if (!imageInfo.IsEmpty && Action.PaintStyle != ActionItemPaintStyle.Caption)
                    {
                        goButton.Image = imageInfo.Image;
                        goButton.ImageLocation = ImageLocation.MiddleCenter;
                        goButton.Caption = string.Empty;
                    }
                    else
                    {
                        goButton.Image = null;
                        goButton.Caption = Action.ShortCaption;
                    }
                    tempControl.Properties.Buttons.Add(goButton);
                    var goButtonField = type.GetField(nameof(goButton), BindingFlags.Instance | BindingFlags.NonPublic);
                    goButtonField.SetValue(this, goButton);
                }

                var eventInfo = tempControl.GetType().GetEvent("PreviewKeyDown");
                var methodInfo = type.GetMethod("result_PreviewKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
                var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
                eventInfo.AddEventHandler(tempControl, handler);

                tempControl.ButtonClick += ControlButtonClick;

                tempControl.KeyDown += ControlKeyDown;

                tempControl.Tag = EasyTestTagHelper.FormatTestAction(Action.Caption);
                tempControl.Name = "Control_" + Guid.NewGuid();
                tempControl.EditValue = Action.Value;
                var controlCreatedField = type.GetField("controlCreated", BindingFlags.Instance | BindingFlags.NonPublic);
                controlCreatedField.SetValue(this, true);
                control = tempControl;
                tempControl = null;
            }
            finally
            {
                if (tempControl != null)
                {
                    tempControl.Dispose();
                }
            }
            return control;
        }

        private void ControlKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ExecuteWithCurrentValue();
            }
        }

        private void ControlButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == goButton)
            {
                this.ExecuteWithCurrentValue();
            }
        }

        private void ExecuteWithCurrentValue()
        {
            Action.DoExecute(Control.Text);
        }
    }
}
