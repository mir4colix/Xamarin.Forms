﻿using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Platform.Handlers;

namespace Xamarin.Platform
{
	public static class PickerExtensions
	{
		public static void UpdateTitle(this NativePicker nativePicker, IPicker picker) =>
			nativePicker.UpdatePicker(picker);
		
		public static void UpdateTitleColor(this NativePicker nativePicker, IPicker picker) =>
			nativePicker.SetTitleColor(picker);
		
		public static void UpdateTextColor(this NativePicker nativePicker, IPicker picker) =>
			nativePicker.UpdateTextColor(picker, nativePicker.TextColor);

		public static void UpdateTextColor(this NativePicker nativePicker, IPicker picker, UIColor? defaultColor)
		{
			var textColor = picker.Color;

			if (textColor.IsDefault || (!picker.IsEnabled))
				nativePicker.TextColor = defaultColor;
			else
				nativePicker.TextColor = textColor.ToNative();

			// HACK This forces the color to update; there's probably a more elegant way to make this happen
			nativePicker.Text = nativePicker.Text;
		}

		public static void UpdateSelectedIndex(this NativePicker nativePicker, IPicker picker) =>
			nativePicker.SetSelectedIndex(picker, picker.SelectedIndex);

		public static void UpdateFont(this NativePicker nativePicker, IPicker picker)
		{
			nativePicker.Font = picker.ToUIFont();
		}

		public static void UpdateCharacterSpacing(this NativePicker nativePicker, IPicker picker)
		{
			nativePicker.SetCharacterSpacing(picker);
		}

		public static void UpdateHorizontalTextAlignment(this NativePicker nativePicker, IPicker picker)
		{
			nativePicker.TextAlignment = picker.HorizontalTextAlignment.ToNativeTextAlignment(EffectiveFlowDirection.Explicit);
		}

		public static void UpdateVerticalTextAlignment(this NativePicker nativePicker, IPicker picker)
		{
			nativePicker.VerticalAlignment = picker.VerticalTextAlignment.ToNativeTextAlignment();
		}

		internal static void UpdatePicker(this NativePicker nativePicker, IPicker picker)
		{
			var selectedIndex = picker.SelectedIndex;
			var items = picker.Items;

			nativePicker.SetTitleColor(picker);

			nativePicker.Text = selectedIndex == -1 || items == null || selectedIndex >= items.Count ? string.Empty : items[selectedIndex];

			var pickerView = nativePicker.UIPickerView;
			pickerView?.ReloadAllComponents();

			if (items == null || items.Count == 0)
				return;

			nativePicker.SetSelectedIndex(picker, selectedIndex);
			nativePicker.SetSelectedItem(picker);
			nativePicker.SetCharacterSpacing(picker);
		}

		internal static void SetTitleColor(this NativePicker nativePicker, IPicker picker)
		{
			var title = picker.Title;

			if (string.IsNullOrEmpty(title))
				return;

			var titleColor = picker.TitleColor;

			nativePicker.UpdateAttributedPlaceholder(new NSAttributedString(title, null, titleColor.ToNative()));
		}

		internal static void SetCharacterSpacing(this NativePicker nativePicker, IPicker picker)
		{
			var textAttr = nativePicker.AttributedText?.AddCharacterSpacing(nativePicker.Text ?? string.Empty, picker.CharacterSpacing);

			if (textAttr != null)
				nativePicker.AttributedText = textAttr;

			var placeHolder = nativePicker.AttributedPlaceholder?.AddCharacterSpacing(picker.Title, picker.CharacterSpacing);

			if (placeHolder != null)
				nativePicker.UpdateAttributedPlaceholder(placeHolder);
		}

		internal static void SetSelectedIndex(this NativePicker nativePicker, IPicker picker, int selectedIndex = 0)
		{
			picker.SelectedIndex = selectedIndex;
			picker.SelectedIndexChanged();

			var pickerView = nativePicker.UIPickerView;

			if (pickerView?.Model is PickerSource source)
			{
				source.SelectedIndex = selectedIndex;
				source.SelectedItem = selectedIndex >= 0 ? picker.Items[selectedIndex] : null;
			}

			pickerView?.Select(Math.Max(selectedIndex, 0), 0, true);
		}

		internal static void SetSelectedItem(this NativePicker nativePicker, IPicker picker)
		{
			if (nativePicker == null)
				return;

			int index = picker.SelectedIndex;

			if (index == -1)
			{
				picker.SelectedItem = null;
				return;
			}

			if (picker.ItemsSource != null)
			{
				picker.SelectedItem = picker.ItemsSource[index];
				return;
			}

			picker.SelectedItem = picker.Items[index];
		}

		internal static void UpdateAttributedPlaceholder(this NativePicker nativePicker, NSAttributedString nsAttributedString)
		{
			nativePicker.AttributedPlaceholder = nsAttributedString;
		}
	}
}