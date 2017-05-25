using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace App1
{
    public partial class ViewController : UIViewController
    {
        private List<NSObject> _removeObjects = new List<NSObject>();

        public ViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // software keyboardを隠す
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                foreach (var subview in this.View.Subviews)
                {
                    subview.ResignFirstResponder();
                }
            }));

            var nsObject = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, n =>
            {
                var cgRectValue = (n.UserInfo[UIKeyboard.FrameBeginUserInfoKey] as NSValue).CGRectValue;
                var d = (n.UserInfo[UIKeyboard.AnimationDurationUserInfoKey] as NSNumber).DoubleValue;

                var active = this.View.Subviews.FirstOrDefault(u => u.IsFirstResponder);
                var diff = this.View.Frame.Height - active?.Frame.Bottom ?? 0f;

                UIView.Animate(d, () =>
                {
                    var transform = CGAffineTransform.MakeTranslation(0f, -(float)Math.Max(cgRectValue.Height - diff, 0f));
                    this.View.Transform = transform;
                });
            });
            this._removeObjects.Add(nsObject);

            nsObject = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, n =>
            {
                var d = (n.UserInfo[UIKeyboard.AnimationDurationUserInfoKey] as NSNumber).DoubleValue;
                UIView.Animate(d, () =>
                {
                    this.View.Transform = CGAffineTransform.MakeIdentity();
                });

            });
            this._removeObjects.Add(nsObject);
        }

        public override void ViewDidAppear(bool animated)
        {
            // TextViewにborder設定。
            foreach (var textView in this.View.Subviews.OfType<UITextView>())
            {
                textView.Layer.BorderColor = new CGColor(0.8f, 0.8f, 0.8f);
                textView.Layer.BorderWidth = 0.5f;
                textView.Layer.CornerRadius = 5;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            this._removeObjects.ForEach(NSNotificationCenter.DefaultCenter.RemoveObserver);
        }
    }
}