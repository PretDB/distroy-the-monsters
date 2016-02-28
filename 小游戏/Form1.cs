using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace 小游戏
{
    //public class InputBox: Form
    //{
    //    public InputBox()
    //    {
    //        TextBox tb_playerName = new TextBox();
    //        Button bt_confirm = new Button();
    //        Label lb_label = new Label();

    //        Point p = new Point(20, 260);
    //        Size s = new Size(this.Width, tb_playerName.Height);
    //        tb_playerName.Size = s;

    //        bt_confirm.Text = "确认";

    //        Size s_Form = new Size(300, 270);
    //        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
    //        this.StartPosition = FormStartPosition.CenterScreen;
    //        this.Size = s_Form;

    //        this.Controls.Add(tb_playerName);
    //        this.Controls.Add(bt_confirm);
    //        this.Controls.Add(lb_label);
            
    //    }
        
    //}
    public partial class Form1 : Form
    {
        /// <summary>
        /// 产生flight方块
        /// 定义存放子弹的动态数组
        /// 定义存放怪物的动态数组
        /// </summary>
        public cube Cube = new cube();
        public System.Collections.ArrayList arylst_bullets = new System.Collections.ArrayList();
        public System.Collections.ArrayList arylst_monsters = new System.Collections.ArrayList();
        public static int windowWidth;
        public static int WindowHeight;
        public static uint total;
        public static uint missed;
        /// <summary>
        /// 更新屏幕时间回调
        /// 
        /// </summary>
        public TimerCallback tcb_updateScreen;
        //public TimerCallback tcb_updateFireAllowed;
        public TimerCallback tcb_generateMonster;

        public System.Threading.Timer tmr_timer;
        //public System.Threading.Timer tmr_fireAllowed;
        public System.Threading.Timer tmr_Monster;

        public bool b_fileAllowed = true;
        public string s_playerName = "default player";

        public static void setPlayerName(string name)
        {
           
        }
        public Form1()
        {
            InitializeComponent();
            Cube.location.X = this.Size.Width / 2;
            Cube.location.Y = this.Height - 5 * this.Cube.size.Height;
            
            
        }

        private void Form1_In(object sender, EventArgs e)
        {
            this.draw += new Draw(Form1_Draw);

            this.KeyDown += new KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new KeyEventHandler(this.Form1_KeyUp);


            MessageBox.Show("欢迎！\r\n提示：左右箭头控制方块，空格键发射,Esc退出\r\n消灭绿色的怪物吧！");
            Form1_Load(sender, e);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            

            Form2 fm2_inoutBox = new Form2();
            fm2_inoutBox.ShowDialog();
            this.s_playerName = fm2_inoutBox.s_Name;

            /*更新屏幕用的语句，更新屏幕依靠的是时钟时间
            */
            this.tcb_updateScreen = new TimerCallback(this.updateScreen);
            this.tmr_timer = new System.Threading.Timer(tcb_updateScreen, null, 0, 20);

            /*驱动产生monster的时钟
             * 
            */
            this.tcb_generateMonster = new TimerCallback(this.generateMonsters);
            this.tmr_Monster = new System.Threading.Timer(tcb_generateMonster, null, 0, 1000);


            windowWidth = this.Size.Width;
            WindowHeight = this.Size.Height;

            /*该语句已经废置不用，原因详见this.updateFireAllowed
             *this.tcb_updateFireAllowed = new TimerCallback(this.updateFireAllowed); 
             * this.tmr_fireAllowed = new System.Threading.Timer(tcb_updateFireAllowed, null, 0, 50);
            */


        }


        private void Write2Log(string name, int score)
        {
            FileStream fs_playLog = new FileStream("score.txt", FileMode.Append);
            StreamWriter sw_playLog = new StreamWriter(fs_playLog);

            sw_playLog.WriteLine("=====================================================");
            sw_playLog.WriteLine("Player Name    :" + name);
            sw_playLog.WriteLine("Score          :" + score.ToString());
            sw_playLog.WriteLine("Time           :" + System.DateTime.Now.ToString());
            sw_playLog.WriteLine("Total Monsters :" + total.ToString());
            sw_playLog.WriteLine("Missed         :" + missed.ToString());
            sw_playLog.WriteLine("=====================================================");
            sw_playLog.WriteLine("\r\n");

            sw_playLog.Close();
            fs_playLog.Close();

        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    Cube.Stop();
                    break;
                case Keys.D:
                case Keys.Right:
                    Cube.Stop();
                    break;
                case Keys.Space:
                    Cube.stopBullet();
                    break;
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    if (Cube.location.X <= 0 || Cube.location.X > this.Width - this.Cube.size.Width)
                    {
                        Cube.Stop();
                        Cube.location.X = 0;
                    }
                    else
                    {
                        
                        Cube.Move(-1, this.Width);
                    }
                    break;
                case Keys.D:
                case Keys.Right:
                    if (Cube.location.X < 0 || Cube.location.X >= this.Width - this.Cube.size.Width)
                    {
                        Cube.Stop();
                        Cube.location.X = this.Width - this.Cube.size.Width;
                    }
                    else
                    {
                        
                        Cube.Move(1, this.Width);
                    }
                    break;
                
                case Keys.Space:
                    if (b_fileAllowed)
                    {
                        Cube.fireBullet(ref this.arylst_bullets);

                    }
                    break;
                case Keys.Escape:
                    this.tmr_timer.Dispose();
                    DialogResult dr_result = MessageBox.Show("退出吗？这么好玩的游戏你竟然退出？", "这么好玩的游戏你竟然退出？", MessageBoxButtons.YesNo);
                    if (dr_result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Write2Log(s_playerName, score);
                        this.Dispose();
                        Application.ExitThread();
                    }
                    else
                    {
                        this.tmr_timer = new System.Threading.Timer(tcb_updateScreen, null, 0, 20);
                    }
                    break;

            }
            
        }


        /// <summary>
        /// 更新开火允许变量，现在已经废置不用。设立这个方法的目的是
        /// 为了解决当有子弹消失的时候提示“集合已更改，枚举可能失效”
        /// 的问题。但是后来查证该问题并非是因此产生。详见
        /// <code>Form1_Draw(object ob)</code>
        /// </summary>
        /// <param name="ob"></param>
        public void updateFireAllowed(object ob)
        {
            if (b_fileAllowed == false)
            {
                b_fileAllowed = true;
            }
            else
            {
                b_fileAllowed = false;
            }
        }
        public void updateScreen(object ob)
        {
            this.draw(this, new EventArgs());
        }

        private void Form1_Draw(object sender, EventArgs e)
        {

            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.BlueViolet);
            Brush b = new SolidBrush(Color.Red);

            g.Clear(Color.Black);

            Cube.draw(ref g);
            g.DrawString("分数：", new Font("微软雅黑", 20), b, new PointF(0f, this.Height - 40));
            g.DrawString(score.ToString(), new Font("微软雅黑", 20), b, new PointF(this.Width / 2, this.Height - 40));
            g.DrawString(this.s_playerName, new Font("微软雅黑", 20), b, new PointF(0f, 0));


            /*创建临时动态数组，防止出现“集合已更改，foreach枚举可能失效
             *创建克隆之后才正常，不然会出现“集合已更改……"，注意一定要使用
             *克隆，否则问题依旧。目前怀疑是使用简单的 = 赋值仅仅是建立了一
             *种类似引用的关系，导致在更改临时数组的时候实际上却更改了先前的
             *动态数组
             */
            System.Collections.ArrayList arylst_tempBullets;
            arylst_tempBullets = (System.Collections.ArrayList)this.arylst_bullets.Clone();

            System.Collections.ArrayList arylst_tempMonsters;
            arylst_tempMonsters = (System.Collections.ArrayList)this.arylst_monsters.Clone();

            foreach (monsters mstr in arylst_tempMonsters)
            {
                foreach (bullet blt in arylst_tempBullets)
                {
                    if (System.Math.Sqrt((blt.location.X - mstr.location.X - 10) * (blt.location.X -mstr.location.X - 10) + (blt.location.Y - mstr.location.Y - 10) * (blt.location.Y - mstr.location.Y - 10)) <= 20)
                    {
                        arylst_bullets.Remove(blt);
                        arylst_monsters.Remove(mstr);
                        score += 10;
                    }
                }

                if (System.Math.Sqrt((Cube.location.X - mstr.location.X) * (Cube.location.X - mstr.location.X) + (Cube.location.Y - mstr.location.Y) * (Cube.location.Y - mstr.location.Y)) <= 20)
                {
                    
                    this.tmr_timer.Dispose();
                    this.tmr_Monster.Dispose();
                    this.drawDie();

                    break;

                }
            }

            foreach (bullet blt in arylst_tempBullets)
            {
                if (blt.b_isAvaliabel)
                {

                    blt.Draw(ref g);
                }
                else
                {
                    arylst_bullets.Remove(blt);
                    
                }
            }
            foreach (monsters mstr in arylst_tempMonsters)
            {
                if (mstr.b_isAvaliabel)
                {
                    mstr.Draw(ref g);
                }
                else
                {
                    arylst_monsters.Remove(mstr);
                    score -= 20;
                    missed++;
                }
            }


            g.Dispose();
        }



        private void drawDie()
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Green, 3f);
            Brush b = new SolidBrush(Color.Gray);
            DialogResult dr_result = MessageBox.Show("你死了", "你死了", MessageBoxButtons.RetryCancel);
            if (dr_result == System.Windows.Forms.DialogResult.Retry)
            {
                //this.tmr_timer = new System.Threading.Timer(tcb_updateScreen, null, 0, 20);
                //this.tmr_Monster = new System.Threading.Timer(tcb_generateMonster, null, 0, 2000);

                arylst_bullets.Clear();
                arylst_monsters.Clear();
                this.Write2Log(this.s_playerName, this.score);
                this.score = 0;
                this.Form1_Load(new object(), new EventArgs());

            }
            else
            {
                Application.Exit();
            }
           
        }

        
        private void generateMonsters(object ob)
        {
            System.Random ran = new Random();

            Point p = new Point(ran.Next(20, this.Size.Width - 20), 20);
            int s = ran.Next(1, 3);

            monsters monster = new monsters(p, s);



            arylst_monsters.Add(monster);

            total++;
        }


    }

    public class cube
    {
        public Point location = new Point(0, 0);
        public Size size;
        public int Speed = 6;
        private int moveDiretcion = 1;
        private TimerCallback tcb_cubeMove;
        private System.Threading.Timer tmr_cubeMove = null;
        private TimerCallback tcb_fireBullet = null;
        private System.Threading.Timer tmr_fireBullet = null;
        private System.Collections.ArrayList arylst_bulletsRef;
        public cube()
        {
            this.size.Width = 10;
            this.size.Height = 10;
            this.tcb_cubeMove = new TimerCallback(this.move);
            this.tcb_fireBullet = new TimerCallback(this.firebullet);
        }
        public cube(Point p)
        {
            this.location = p;
            this.tcb_cubeMove = new TimerCallback(this.move);
            this.tcb_fireBullet = new TimerCallback(this.firebullet);
        }
        public void draw(ref Graphics g)
        {
            Brush b = new SolidBrush(Color.BlueViolet);
            Pen p = new Pen(Color.Green);
            g.FillRectangle(b, this.location.X, this.location.Y, this.size.Width, this.size.Height);
        }

        private void move(object ob)
        {
            this.location.X += Speed * this.moveDiretcion;
        }
        public void Move(int dire, int edge)
        {
            this.moveDiretcion = dire;
            if ((this.location.X + this.size.Width <= edge && this.location.X >= 0) && ((this.tmr_cubeMove == null)))
            {
                this.tmr_cubeMove = new System.Threading.Timer(tcb_cubeMove, null, 0, 20);
            }

        }
        public void Stop()
        {
            if (this.tmr_cubeMove != null)
            {
                this.tmr_cubeMove.Dispose();
                this.tmr_cubeMove = null;
            }
        }


        public void fireBullet(ref System.Collections.ArrayList arylst)
        {
            if (this.tmr_fireBullet == null)
            {
                this.arylst_bulletsRef = arylst;
                this.tmr_fireBullet = new System.Threading.Timer(tcb_fireBullet, null, 0, 260);
            }
        }
        private void firebullet(object ob)
        {
            bullet bullet_newBullet = new bullet(this.location);
            arylst_bulletsRef.Add(bullet_newBullet);

        }
        public void stopBullet()
        {
            if (this.tmr_fireBullet != null)
            {
                this.tmr_fireBullet.Dispose();
                this.tmr_fireBullet = null;
            }
        }

    }



    /// <summary>
    /// 子弹类，每一个实例有自己的timer，用来更新自己的位置
    /// </summary>
    public class bullet
    {
        public Point location = new Point();
        public int Speed = 20;

        //时间回调用来自动更新子弹的位置，该回调在产生实例的时候自动新建
        public TimerCallback tcb_bullet;
        public System.Threading.Timer tmr_timer;
        public bullet(Point p)
        {
            this.location = new Point(p.X + 3, p.Y);

            this.tcb_bullet = new TimerCallback(this.Move);
            this.tmr_timer = new System.Threading.Timer(tcb_bullet, null, 0, 20);
        }



        /// <summary>
        /// 返回该子弹是否有效，location.Y <= 0即视作失效，应当进行销毁处理
        /// </summary>
        /// <returns></returns>
        private bool b_Avaliabel()
        {
            return (!(this.location.Y <= 0));
        }

        public bool b_isAvaliabel
        {
            get
            {
                return this.b_Avaliabel();
            }
            set
            {
                b_isAvaliabel = value;
            }
        }
        
        public void Draw(ref Graphics g)
        {
            if(b_isAvaliabel)
            {
                Brush b = new SolidBrush(Color.Red);
                g.FillEllipse(b, this.location.X, this.location.Y, 4f, 4f);
            }
        }

        public void Move(object ob)
        {
            if (b_isAvaliabel)
            {
                this.location.Y -= Speed;
            }
            
        }
    }




    /// <summary>
    /// 和bullet类似
    /// monster移动的方法也是通过一个时钟回调
    /// </summary>
    public class monsters
    {
        public Point location = new Point();
        public int Speed = 5;

        
        public bool b_isAvaliabel
        {
            get
            {
                return this.b_Avaliabel();
            }
            set
            {
                b_isAvaliabel = value;
            }
            
        }

        public TimerCallback tcb_monster;
        public System.Threading.Timer tmr_timer;

       
        public monsters(Point p, int speed)
        {
            this.location = new Point(p.X + 3, p.Y);
            this.Speed = speed;

            this.tcb_monster = new TimerCallback(this.Move);
            this.tmr_timer = new System.Threading.Timer(tcb_monster, null, 0, 20);
        }



        /// <summary>
        /// 返回该monster是否有效，location.Y >= this.height即视作失效，应当进行销毁处理
        /// </summary>
        /// <returns></returns>
        private bool b_Avaliabel()
        {
            return (!(this.location.Y >= Form1.WindowHeight - 30));
        }

        
        
        public void Draw(ref Graphics g)
        {
            if(b_isAvaliabel)
            {
                Brush b = new SolidBrush(Color.Green);
                g.FillPie(b, this.location.X, this.location.Y, 20, 20, 60, -300);

            }
        }

        public void Move(object ob)
        {
            if (b_isAvaliabel)
            {
                this.location.Y += Speed;
            }

        }

        public void Die()
        {
            this.b_isAvaliabel = false;
        }
    }

}
