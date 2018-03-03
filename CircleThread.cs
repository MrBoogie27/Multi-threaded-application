using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
namespace OC6
{
    public class RenderThread
    {
        string _priority;
        PictureBox _childForm; //для взятия из формы
        Thread _thread = null; //поток
        public Color _color = new Color();//цвет
        Brush _brush = null;
        int _radius; //радиус
        int _sleepDuration; //время задержки между рисованием
        private Control _parent; //элемент для размеров окна
        Random _random = new Random(); //для рандомного положения
        public string _name; //имя для поиска и вывода
        Size _parentSize; //размер окна
        public readonly ManualResetEvent _pauseEvent; //для приостановки потока
        public readonly ManualResetEvent _exitEvent; //для закрытия поток
        readonly object _clientSizeChangeLocker; //для создания КС
        public RenderThread(PictureBox childForm, Control parent, Thread thread, Color color, int radius, int i, int sleepDuration)
        {
            _priority = "Нормальный";
            _sleepDuration = sleepDuration;
            _childForm = childForm;
            _parent = parent;
            _thread = thread;
            _color = color;
            _brush= new SolidBrush(_color);
            _radius = radius;
            _name = "Thread" + i;
            _parentSize = _parent.ClientSize;
            _exitEvent = new ManualResetEvent(false);
            _pauseEvent = new ManualResetEvent(false);
            _clientSizeChangeLocker = new object();
            _thread = new Thread(this.DrawCircle) { Name = _name };
            _thread.Priority = ThreadPriority.Normal;
        }
        public void ParentResize(object sender, EventArgs e)
        {
            _parentSize = (sender as Control).ClientSize;
            _childForm.ClientSize = new Size(_parentSize.Width-200, _parentSize.Height-10);
        }
        public string Priority
        {
            get { return _priority; }
            set {
                _priority = value;
                switch (_priority)
                {
                    case "Высокий": _thread.Priority = ThreadPriority.Highest;break;
                    case "Выше среднего": _thread.Priority = ThreadPriority.AboveNormal; break;
                    case "Нормальный": _thread.Priority = ThreadPriority.Normal; break;
                    case "Ниже среднего": _thread.Priority = ThreadPriority.BelowNormal; break;
                    case "Низкий": _thread.Priority = ThreadPriority.Lowest; break;
                }
                }
        }
        public int SleepDuration
        { get { return _sleepDuration; } set { _sleepDuration = value; } }
        public int Radius
        { get { return _radius; } set { _radius = value; } }
        public Color UsedColor
        { get { return _color; }
          set {
                lock (_clientSizeChangeLocker)
                {
                    _color = value;
                    Brush k = _brush;
                    _brush = new SolidBrush(_color);
                    if(k != null)
                        k.Dispose();

                }
              }
        }
        public void Suspend()
        {
            _pauseEvent.Set();
        }
        public void Resume()
        {
            _pauseEvent.Reset();
        }
        public void Run()
        {
            _thread.Start();
        }
        public void Dispose()
        {
            _exitEvent.Set();
            Resume();
            _thread.Join();
            _exitEvent.Close();
        }
        public void DrawCircle()
        {
          
            do
            {
                lock (_clientSizeChangeLocker)
                    for (int i = 0; i < 100; i++)
                {
                    Point point = new Point { X = _random.Next(0, _childForm.ClientSize.Width - _radius), Y = _random.Next(0, _childForm.ClientSize.Height - _radius) };

                    Graphics g = _childForm.CreateGraphics();
                    g.FillEllipse(_brush, point.X, point.Y, _radius, _radius);
                    g.Dispose();
                }
                if (!_thread.IsAlive)
                    break;
               // Thread.Sleep(0);
                while (_pauseEvent.WaitOne(0)) ;
            }
            while (!_exitEvent.WaitOne(_sleepDuration));
        }
    }
}