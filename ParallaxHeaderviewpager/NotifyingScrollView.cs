//
//  NotifyingScrollView.cs
//
//  Author:
//       welcome <>
//
//  Copyright (c) 2015 welcome
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using Android.Widget;
using Android.Content;
using Android.Util;

namespace ParallaxHeaderviewpager
{
	public class NotifyingScrollView : ScrollView
	{
		public interface OnScrollChangedListener {
			void onScrollChanged(ScrollView who, int l, int t, int oldl, int oldt);
		}

		private OnScrollChangedListener mOnScrollChangedListener;

		public NotifyingScrollView(Context context):base(context) {
		}

		public NotifyingScrollView(Context context, IAttributeSet attrs):base(context, attrs) {

		}

		public NotifyingScrollView(Context context, IAttributeSet attrs, int defStyle):base(context, attrs, defStyle) {

		}

		protected override void OnScrollChanged(int l, int t, int oldl, int oldt) {
			base.OnScrollChanged(l, t, oldl, oldt);
			if (mOnScrollChangedListener != null) {
				mOnScrollChangedListener.onScrollChanged(this, l, t, oldl, oldt);
			}
		}

		public void SetOnScrollChangedListener(OnScrollChangedListener listener) {
			mOnScrollChangedListener = listener;
		}

	}
}

