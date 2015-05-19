//
//  CustomSlidingTabIndicator.cs
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
using Android.Support.V4.View;
using Android.Graphics;
using Android.Content;
using Android.Util;
using Android.Content.Res;
using Java.Lang;
using Android.Views;
using Android.OS;
using Java.Interop;

namespace ParallaxHeaderviewpager
{
	public class CustomSlidingTabIndicator : HorizontalScrollView, Android.Support.V4.View.ViewPager.IOnPageChangeListener, ViewTreeObserver.IOnGlobalLayoutListener, View.IOnClickListener
	{
		private static string TAG = "CustomTabSlidingIndicator";

		public interface IconTabProvider
		{
			int GetPageIconResId (int position);
		}

		private static int[] ATTRS = new int[] {Android.Resource.Attribute.TextSize,
			Android.Resource.Attribute.TextColor
		};

		private LinearLayout mTabsContainer;
		private ViewPager mViewPager;
		private LinearLayout.LayoutParams mDefaultTabLayoutParams;
		private LinearLayout.LayoutParams mExpandedTabLayoutParams;

		private ViewPager.IOnPageChangeListener mListener;

		private int mTabCount;

		private int mCurrentPosition = 0;
		private float mCurrentPositionOffset = 0.0f;

		private Paint mRectPaint;
		private Paint mDividerPaint;

		private int mScrollOffset = 52;
		private int mIndicatorHeight = 8;
		private int mUnderlineHeight = 2;
		private int mDividerPadding = 12;
		private int mTabSidePadding = 24;
		private int mTabTopBtmPadding = 0;
		private int mDividerWidth = 1;

		private int mTabTextSize = 12;
		private Color mTabTextColor;
		private Typeface mTabTypeface = null;
		private TypefaceStyle mTabTypefaceStyle = TypefaceStyle.Bold;

		private Color mIndicatorColor;
		private Color mUnderlineColor;
		private Color mDividerColor;

		private int mTabBackgroundResId = Resource.Drawable.sliding_bar_bg;

		private bool mShouldExpand = false;
		private bool mTextAllCap = false;

		private int mLastScrollX = 0;

		public CustomSlidingTabIndicator (Context ctx) : this (ctx, null)
		{
		}

		public CustomSlidingTabIndicator (Context ctx, IAttributeSet attrs) : this (ctx, attrs, 0)
		{
		}

		public CustomSlidingTabIndicator (Context ctx, IAttributeSet attrs, int defStyle) : base (ctx, attrs, defStyle)
		{
			//Request HorizontalScrollView to stretch its content to fill the viewport
			FillViewport = true;

			//This view will not do any drawing on its own, clear this flag if you override onDraw()
			SetWillNotDraw (false);

			//Layout to hold all the tabs
			mTabsContainer = new LinearLayout (ctx);
			mTabsContainer.Orientation = Android.Widget.Orientation.Horizontal;
			mTabsContainer.LayoutParameters = new LayoutParams (LayoutParams.MatchParent, LayoutParams.MatchParent);

			//Add the container to HorizontalScrollView as its only child view
			AddView (mTabsContainer);

			//Convert the dimensions to DP
			DisplayMetrics dm = Resources.DisplayMetrics;
			mScrollOffset = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, mScrollOffset, dm);
			mIndicatorHeight = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, mIndicatorHeight, dm);
			mUnderlineHeight = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, mUnderlineHeight, dm);
			mDividerPadding = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, mDividerPadding, dm);
			mTabSidePadding = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, mTabSidePadding, dm);
			mTabTopBtmPadding = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, mTabTopBtmPadding, dm);
			mDividerWidth = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, mDividerWidth, dm);
			mTabTextSize = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, mTabTextSize, dm);

			//Get system attrs (android:textSize & android:textColor)
			TypedArray typedArray = ctx.ObtainStyledAttributes (attrs, ATTRS);

			mTabTextSize = typedArray.GetDimensionPixelSize (0, mTabTextSize);
			mTabTextColor = typedArray.GetColor (1, mTabTextColor);

			typedArray.Recycle ();

			//Get custom attrs
			typedArray = ctx.ObtainStyledAttributes (attrs, Resource.Styleable.CustomSlidingTabIndicator);

			mIndicatorColor = typedArray.GetColor (Resource.Styleable.CustomSlidingTabIndicator_STIindicatorColor, mIndicatorColor);
			mUnderlineColor = typedArray.GetColor (Resource.Styleable.CustomSlidingTabIndicator_STIunderlineColor, mUnderlineColor);
			mDividerColor = typedArray.GetColor (Resource.Styleable.CustomSlidingTabIndicator_STIdividerColor, mDividerColor);
			mTabTextColor = typedArray.GetColor (Resource.Styleable.CustomSlidingTabIndicator_STItextColor, mTabTextColor);
			mIndicatorHeight = typedArray.GetDimensionPixelSize (Resource.Styleable.CustomSlidingTabIndicator_STIindicatorHeight, mIndicatorHeight);
			mUnderlineHeight = typedArray.GetDimensionPixelSize (Resource.Styleable.CustomSlidingTabIndicator_STIunderlineHeight, mUnderlineHeight);
			mDividerPadding = typedArray.GetDimensionPixelSize (Resource.Styleable.CustomSlidingTabIndicator_STIdividersPadding, mDividerPadding);
			mTabSidePadding = typedArray.GetDimensionPixelSize (Resource.Styleable.CustomSlidingTabIndicator_STItabLeftRightPadding, mTabSidePadding);
			mScrollOffset = typedArray.GetDimensionPixelSize (Resource.Styleable.CustomSlidingTabIndicator_STIscrollOffSet, mScrollOffset);
			mTabTextSize = typedArray.GetDimensionPixelSize (Resource.Styleable.CustomSlidingTabIndicator_STItabTextSize, mTabTextSize);
			mTabBackgroundResId = typedArray.GetResourceId (Resource.Styleable.CustomSlidingTabIndicator_STItabBackground, mTabBackgroundResId);
			mShouldExpand = typedArray.GetBoolean (Resource.Styleable.CustomSlidingTabIndicator_STIshouldExpand, mShouldExpand);
			mTextAllCap = typedArray.GetBoolean (Resource.Styleable.CustomSlidingTabIndicator_STItextCaps, mTextAllCap);
			mTabTopBtmPadding = typedArray.GetDimensionPixelSize (Resource.Styleable.CustomSlidingTabIndicator_STItabTopBtmPadding, mTabTopBtmPadding);

			typedArray.Recycle ();

			//Paint to draw the rectangle box
			mRectPaint = new Paint ();
			mRectPaint.AntiAlias = true;
			mRectPaint.SetStyle (Paint.Style.Fill);

			//Paint to draw the divider
			mDividerPaint = new Paint ();
			mDividerPaint.AntiAlias = true;
			mDividerPaint.StrokeWidth = mDividerWidth;

			//Default: width = wrap_content, height = match_parent
			mDefaultTabLayoutParams = new LinearLayout.LayoutParams (LayoutParams.WrapContent, LayoutParams.MatchParent);

			//Expanded: width = 0, height = match_parent, weight = 1.0f
			mExpandedTabLayoutParams = new LinearLayout.LayoutParams (0, LayoutParams.MatchParent, 1.0f);
		}

		public void SetViewPager (ViewPager pager)
		{
			mViewPager = pager;

			if (pager.Adapter == null) {
				throw new IllegalStateException ("ViewPager does not have an adapter instance");
			}
			pager.SetOnPageChangeListener (this);

			NotifyDataSetChange ();
		}

		public void SetPageListener (ViewPager.IOnPageChangeListener listener)
		{
			mListener = listener;
		}

		public void NotifyDataSetChange ()
		{
			// Remove all the views within the container
			mTabsContainer.RemoveAllViews ();

			mTabCount = mViewPager.Adapter.Count;

			for (int i = 0; i < mTabCount; ++i) {

				if (mViewPager.Adapter is IconTabProvider) {
					AddIconTab (i, ((IconTabProvider)mViewPager.Adapter).GetPageIconResId (i));
				} else {
					AddTextTab (i, mViewPager.Adapter.GetPageTitle (i).ToString ());
				}
			}

			UpdateTabStyles ();


		}

		private void UpdateTabStyles ()
		{
			//Custom view needs no further styling here, all done in xml
			if (mTabBackgroundResId != Resource.Drawable.sliding_bar_bg) {
				return;
			}

			for (int i = 0; i < mTabCount; ++i) {
				View view = mTabsContainer.GetChildAt (i);

				view.SetBackgroundResource (mTabBackgroundResId);

				if (view is TextView) {
					TextView tab = (TextView)view;
					tab.SetTextSize (ComplexUnitType.Px, mTabTextSize);
					tab.SetTypeface (mTabTypeface, mTabTypefaceStyle);
					tab.SetTextColor (mTabTextColor);

					// setAllCaps() is only available from API 14, so the upper case is made manually if we are on a
					// pre-ICS-build
					if (mTextAllCap) {
						if ((int)Build.VERSION.SdkInt >= 14) {
							tab.SetAllCaps (true);
						} else {
							tab.Text = tab.Text.ToUpperInvariant ();
						}
					}
				}
			}
		}

		protected override void OnDraw(Canvas canvas) {
			base.OnDraw(canvas);

			//Check if view is currently in edit mode
			if (IsInEditMode || mTabCount == 0) return;

			//Get the height of this view
			int height = Height;

			//Draw UnderLine
			mRectPaint.Color = mUnderlineColor;
			canvas.DrawRect(0, height - mUnderlineHeight, mTabsContainer.Width, height, mRectPaint);

			//Draw indicator line
			mRectPaint.Color = mIndicatorColor;

			//Default: draw line below current tab
			View currentTab = mTabsContainer.GetChildAt(mCurrentPosition);

			//Get left and right position of tab relative to its parent
			float lineLeft = currentTab.Left;
			float lineRight = currentTab.Right;

			//If there is an offset, start interpolating left and right coordinates between current and next tab
			if (mCurrentPositionOffset > 0f && mCurrentPosition < mTabCount - 1) {
				View nextTab = mTabsContainer.GetChildAt(mCurrentPosition + 1);
				float nextTabLeft = nextTab.Left;
				float nextTabRight = nextTab.Right;

				lineLeft = (mCurrentPositionOffset * nextTabLeft + (1f - mCurrentPositionOffset) * lineLeft);
				lineRight = (mCurrentPositionOffset * nextTabRight + (1f - mCurrentPositionOffset) * lineRight);
			}

			canvas.DrawRect(lineLeft, height - mIndicatorHeight, lineRight, height, mRectPaint);

			//draw divider
			mDividerPaint.Color = mDividerColor;
			for (int i = 0; i < mTabCount - 1; ++i) {
				View tab = mTabsContainer.GetChildAt(i);
				canvas.DrawLine(tab.Right, mDividerPadding, tab.Right, height - mDividerPadding, mDividerPaint);
			}
		}

		private void AddTextTab (int position, string title)
		{

			if (mTabBackgroundResId != Resource.Drawable.sliding_bar_bg) {
				View view = LayoutInflater.From (Context).Inflate (mTabBackgroundResId, null);

				if (view is ViewGroup) {
					int numberOfChild = ((ViewGroup)view).ChildCount;

					if (numberOfChild > 0) {
						// TextView must always be the first child
						TextView titleView = (TextView)((ViewGroup)view).GetChildAt (0);
						titleView.Text = title;
						AddTab (position, view);
					}
				} else if (view is TextView) {
					TextView titleTextView = (TextView)view;
					titleTextView.Text = title;
					AddTab (position, titleTextView);
				}

			} else {
				TextView tab = new TextView (Context);
				tab.Text = title;
				tab.Gravity = GravityFlags.Center;
				tab.SetSingleLine ();

				AddTab (position, tab);
			}
		}

		private void AddIconTab (int position, int resId)
		{
			ImageButton tab = new ImageButton (Context);
			tab.SetImageResource (resId);

			AddTab (position, tab);
		}

		private void AddTab (int position, View tab)
		{
			tab.Tag = position;
			tab.SetOnClickListener (this);

			tab.SetPadding (mTabSidePadding, mTabTopBtmPadding, mTabSidePadding, mTabTopBtmPadding);
			mTabsContainer.AddView (tab, position, mShouldExpand ? mExpandedTabLayoutParams : mDefaultTabLayoutParams);
		}

		public void OnClick (View v)
		{
			mViewPager.CurrentItem = (int)v.Tag;
		}

		public void OnGlobalLayout ()
		{
			if ((int)Build.VERSION.SdkInt < 16) {
				ViewTreeObserver.RemoveGlobalOnLayoutListener(this);
			} else {
				ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
			}

			mCurrentPosition = mViewPager.CurrentItem;
			ScrollToChild(mCurrentPosition, 0);
		}

		private void ScrollToChild (int position, int offset)
		{
			if (mTabCount == 0)
				return;

			//The new x position of the indicator when scrolling occurs
			int newScrollX = mTabsContainer.GetChildAt (position).Left + offset;

			if (position > 0 || offset > 0) {
				newScrollX -= mScrollOffset;
			}

			if (newScrollX != mLastScrollX) {
				mLastScrollX = newScrollX;
				//Move the horizontal scroll view
				//Y coordinate is zero
				ScrollTo (newScrollX, 0);
			}

			if (offset == 0) {
				SetTextColorChange (mCurrentPosition);
			}
		}

		private void SetTextColorChange (int position)
		{
			int childCount = mTabsContainer.ChildCount;
			for (int i = 0; i < childCount; i++) {
				mTabsContainer.GetChildAt (i).Activated = false;
			}
			mTabsContainer.GetChildAt (position).Activated = true;
			//        if (position > 0)             mTabsContainer.getChildAt(position - 1).setActivated(false);
			//        if (position < mTabCount - 1) mTabsContainer.getChildAt(position + 1).setActivated(false);
		}



		public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
		{
			mCurrentPosition = position;
			mCurrentPositionOffset = positionOffset;
			ScrollToChild (position, (int)(positionOffset * mTabsContainer.GetChildAt (position).Width));

			//Redraw the indicator constantly as the scroll moves
			//Invalidate() will trigger a onDraw()
			Invalidate ();

			if (mListener != null) {
				mListener.OnPageScrolled (position, positionOffset, positionOffsetPixels);
			}
		}

		public void OnPageScrollStateChanged (int state)
		{
			if (state == ViewPager.ScrollStateIdle) {
				ScrollToChild (mViewPager.CurrentItem, 0);
			}

			if (mListener != null) {
				mListener.OnPageScrollStateChanged (state);
			}
		}

		public void OnPageSelected (int position)
		{
			if (mListener != null) {
				mListener.OnPageSelected (position);
			}
		}

		public LinearLayout getTabsContainer() {
			return mTabsContainer;
		}

		public View getViewOfPosition(int position) {
			View view = null;
			if (mTabsContainer.ChildCount > position) {
				view =  mTabsContainer.GetChildAt(position);
			}
			return view;
		}

		protected override void OnRestoreInstanceState(IParcelable state) {
			SavedState savedState = (SavedState) state;
			base.OnRestoreInstanceState(savedState.SuperState);
			mCurrentPosition = savedState.CurrentPosition;

			//Request a re-measure and redraw
			RequestLayout();
		}

		protected override IParcelable OnSaveInstanceState()
		{
			IParcelable superState = base.OnSaveInstanceState();
			SavedState savedState = new SavedState(superState);
			savedState.CurrentPosition = mCurrentPosition;
			return savedState;
		}

		public class SavedState : BaseSavedState {
			public int CurrentPosition { get; set; }

			public SavedState(IParcelable superState):base(superState) {
			}

			private SavedState(Parcel source):base(source) {
				CurrentPosition = source.ReadInt();
			}

			public override  void WriteToParcel(Parcel dest, ParcelableWriteFlags  flags){

				base.WriteToParcel(dest, flags);
				dest.WriteInt(CurrentPosition);
			}

			[ExportField("CREATOR")]
			static SavedStateCreator InitializeCreator()
			{
				return new SavedStateCreator();
			}

			class SavedStateCreator : Java.Lang.Object, IParcelableCreator
			{

				#region IParcelableCreator Members

				public Java.Lang.Object CreateFromParcel(Parcel source)
				{
					return new SavedState(source);
				}

				public Java.Lang.Object[] NewArray(int size)
				{
					return new SavedState[size];
				}

				#endregion
			}
		}
	}
}

