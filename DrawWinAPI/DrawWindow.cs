using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;


namespace DrawWinAPI
{
    public class DrawWindow : HwndHost
    {
        #region fields
        private List<WinNative.POINT> _apt = new List<WinNative.POINT>();
        private int _cpt;
        private readonly int _hostHeight;
        private readonly int _hostWidth;
        private IntPtr _hwndHost;
        private const string _windowClass = "DrawWindow";

        #endregion

        #region Constructor
        public DrawWindow(double height, double width)
        {
            _hostHeight = (int)height;
            _hostWidth = (int)width;
            
            
        }
        #endregion 

        #region Methods
        private void RegisterWindowClass()
        {
            var wndClass = WinNative.WNDCLASSEX.Build();
            wndClass.hInstance = IntPtr.Zero;
            wndClass.lpfnWndProc = WinNative.DefaultWindowProc;
            wndClass.lpszClassName = _windowClass;
            wndClass.hbrBackground = WinNative.GetStockObject(WinNative.StockObjects.WHITE_BRUSH);
            wndClass.cbClsExtra = 0;
            wndClass.cbWndExtra = 0;
            wndClass.style = WinNative.CS_VREDRAW | WinNative.CS_HREDRAW;
            wndClass.hCursor = WinNative.LoadCursor(IntPtr.Zero, WinNative.IDC_ARROW);

            WinNative.RegisterClassEx(ref wndClass);
        }


        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {

            _hwndHost = IntPtr.Zero;

            RegisterWindowClass();


            _hwndHost = WinNative.CreateWindowEx(0, _windowClass, "",
                WinNative.WS_CHILD | WinNative.WS_VISIBLE,
                0, 0,
                _hostHeight, _hostWidth,
                hwndParent.Handle,
                (IntPtr)WinNative.HOST_ID,
                IntPtr.Zero,
                IntPtr.Zero);

            return new HandleRef(this, _hwndHost);
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            int cxClient, cyClient;
            
            handled = true;
           
            switch (msg)
            {
                case WinNative.WM_SIZE:
                    cxClient = WinNative.LOWORD(lParam);
                    cyClient = WinNative.HIWORD(lParam);
                    return IntPtr.Zero;
                case WinNative.WM_LBUTTONDOWN:
                    if (_cpt != 0)
                    {
                        _apt.Clear();
                    }
                    _cpt = 1;

                    //store the location of the mouse. This will be the start of the new polyline
                    WinNative.POINT pt = new WinNative.POINT();
                    pt.x = WinNative.LOWORD(lParam);
                    pt.y = WinNative.HIWORD(lParam);
                    _apt.Add(pt);
                    WinNative.InvalidateRect(hwnd,IntPtr.Zero, true);
                    return IntPtr.Zero;


                case WinNative.WM_MOUSEMOVE:
                    if ((WinNative.LOWORD(wParam) & WinNative.MK_LBUTTON) > 0)
                    {
                        var dc = WinNative.GetWindowDC(hwnd);
                        var hpen = WinNative.GetStockObject(WinNative.StockObjects.WHITE_PEN);
                        var ppen = WinNative.SelectObject(dc, hpen);
                        WinNative.POINT[] pts;
                        if (_cpt > 1)
                        {
                            pts = new WinNative.POINT[_apt.Count];
                            for (int i = 0; i < _apt.Count; i++)
                            {
                                pts[i] = new WinNative.POINT() { x = _apt[i].x, y = _apt[i].y };
                            }
                            WinNative.MoveToEx(dc, pts[0].x, pts[0].y, IntPtr.Zero);
                            WinNative.PolylineTo(dc, pts, (uint)_apt.Count);
                        }

                        WinNative.POINT tp = new WinNative.POINT();
                        tp.x = WinNative.LOWORD(lParam);
                        tp.y = WinNative.HIWORD(lParam);
                        this._apt.Add(tp);
                        _cpt += 1;

                        WinNative.SelectObject(dc, ppen);
                        pts = new WinNative.POINT[_apt.Count];
                        for (int i = 0; i < _apt.Count; i++)
                        {
                            pts[i] = new WinNative.POINT() { x = _apt[i].x, y = _apt[i].y };
                        }
                        WinNative.MoveToEx(dc, pts[0].x, pts[0].y, IntPtr.Zero);
                        WinNative.PolylineTo(dc, pts, (uint)_apt.Count);
                        WinNative.ReleaseDC(dc, hwnd);

                    }
                    return IntPtr.Zero;

                case WinNative.WM_PAINT:

                    if (_cpt > 1)
                    {
                        WinNative.PAINTSTRUCT lpPaint = new WinNative.PAINTSTRUCT();
                        var hdc = WinNative.BeginPaint(hwnd, out lpPaint);

                        var pts = new WinNative.POINT[_apt.Count];
                        for (int i = 0; i < _apt.Count; i++)
                        {
                            pts[i] = new WinNative.POINT() { x = _apt[i].x, y = _apt[i].y };
                        }
                        WinNative.MoveToEx(hdc, pts[0].x, pts[0].y, IntPtr.Zero);
                        WinNative.PolylineTo(hdc, pts, (uint)_apt.Count);

                        WinNative.EndPaint(hwnd, ref lpPaint);
                    }

                    return IntPtr.Zero;

            }

            //for unhandled events
            return WinNative.DefWindowProc(hwnd, (uint)msg, wParam, lParam);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            WinNative.DestroyWindow(hwnd.Handle);
            _hwndHost = IntPtr.Zero;
        }
        #endregion
    }
}
