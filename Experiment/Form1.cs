using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Experiment
{
    public partial class Form1 : Form
    {
        private enum TestType { Practice, Test1, Test2 };
        private TestType currentType;
        private int prevTrial = 2;
        private int currentTrial = 2;            // 0 - Test Trial ; 1 - Control Trial
        private int currentQuiz = 0;             // 0 - Prime      ; 1 - Probe
        private int currentFrac = -1;
        private int prevPrimeFrac = -1;
        private int prevProbeFrac = -1;

        private bool isStart = true;
        private bool isDoubleTestOK = true;
        private bool isDoubleControlOK = true;
        private bool isTesting = false;

        private int countTrial = 24;
        private int countTest = 12;
        private int countControl = 12;

        private Stopwatch swTest = new Stopwatch();

        private Random rnd = new Random();

        private List<Quiz> PrimeQuiz1 = new List<Quiz>();
        private List<Quiz> ProbeQuiz1 = new List<Quiz>();

        private int countParticipant = 0;
        private float[] result = new float[96];
        private int testprimerw;
        private int testproberw;
        private int testprimert;
        private int testprobert;
        private int controlprimerw;
        private int controlproberw;
        private int controlprimert;
        private int controlprobert;

        public Form1()
        {
            InitializeComponent();
        }

        //프로그램 실행
        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

            labelFrac1.Font = new Font(labelFrac1.Font.Name, labelFrac1.Font.SizeInPoints * (this.Size.Height / 2250F)); //1056
            labelFrac2.Font = new Font(labelFrac2.Font.Name, labelFrac2.Font.SizeInPoints * (this.Size.Height / 2250F));
            labelFrac1.Width = Convert.ToInt32(labelFrac1.Width * (this.Size.Width / 2000F));
            labelFrac2.Width = Convert.ToInt32(labelFrac2.Width * (this.Size.Width / 2000F));
            labelPlus.Font = new Font(labelPlus.Font.Name, labelPlus.Font.SizeInPoints * (this.Size.Height / 2250F));
            //picboxPlus.Image = Properties.Resources.plus;
            this.MaximizeBox = false;

            panelResult.Hide();
            panelFraction.Hide();
            panelPlus.Hide();
            panelReadyTest.Hide();
            panelMenu.Show();
            panelMenu.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        //연습, 테스트1, 테스트2 버튼 클릭
        private void btnPractice_Click(object sender, EventArgs e)
        {
            Button btnClicked = (Button)sender;
            
            if (btnClicked.Name == "btnPractice")
            {
                currentType = TestType.Practice;

                panelMenu.Hide();
                panelMenu.Dock = System.Windows.Forms.DockStyle.None;
                panelReadyTest.Show();
                panelReadyTest.Dock = System.Windows.Forms.DockStyle.Fill;

                labelTestType.Text = "연습";
                labelPracticeDescip.Show();
                labelTestDescrip.Hide();
                labelTestName.Hide();
                tboxName.Hide();
            }
            else
            {
                if (btnClicked.Name == "btnTest1")
                {
                    currentType = TestType.Test1;
                    labelTestType.Text = "유형 1";
                }                 
                else
                {
                    currentType = TestType.Test2;
                    labelTestType.Text = "유형 2";
                }
                    

                panelMenu.Hide();
                panelMenu.Dock = System.Windows.Forms.DockStyle.None;
                panelReadyTest.Show();
                panelReadyTest.Dock = System.Windows.Forms.DockStyle.Fill;

                labelPracticeDescip.Hide();
                labelTestDescrip.Show();
                labelTestName.Show();
                tboxName.Show();
                tboxName.Text = "";
                tboxName.Focus();
            }
        }

        //시작하기 버튼
        private void btnStart_Click(object sender, EventArgs e)
        {           
            if ((currentType != TestType.Practice) && (string.IsNullOrWhiteSpace(tboxName.Text)))
            {
                MessageBox.Show("이름을 입력해주세요.", "오류");
                tboxName.Focus();
                return;
            }

            MenuHome.Enabled = false;
            MenuForceStop.Enabled = true;
            MenuShowResult.Enabled = false;

            InitializeTest();

            StartTesting();
        }

        //테스트
        private void StartTesting()
        {
            //초기화
            isStart = true;
            isDoubleTestOK = true;
            isDoubleControlOK = true;
            prevTrial = 2;
            currentTrial = 2;    
            currentQuiz = 0;             
            currentFrac = -1;
            prevPrimeFrac = -1;
            prevProbeFrac = -1;
            swTest.Reset();

            testprimerw = 0;
            testproberw = 12;
            testprimert = 24;
            testprobert = 36;
            controlprimerw = 48;
            controlproberw = 60;
            controlprimert = 72;
            controlprobert = 84;

            for (int i = 0; i < 96; i++)
            {
                result[i] = 0;
            }

            if (currentType == TestType.Practice)
            {
                countTrial = 3;
                countTest = 2;
                countControl = 1;
            }
            else
            {
                countTrial = 24;                           //시행 수 지정---------------
                countTest = 12;
                countControl = 12;
            }
            


            //화면준비
            panelReadyTest.Dock = System.Windows.Forms.DockStyle.None;
            panelReadyTest.Hide();

            ShowPlus();  
        }

        private void ShowPlus()
        {
            panelFraction.Dock = System.Windows.Forms.DockStyle.None;
            panelFraction.Hide();
            panelPlus.Show();
            panelPlus.Dock = System.Windows.Forms.DockStyle.Fill;

            if (isStart)
            {
                TimerPlus.Interval = 500;
                isStart = false;
            }
            else
                TimerPlus.Interval = 2000;

            //화면에 플러스 띄우기
            TimerPlus.Start();
        }

        //플러스 타이머
        private void TimerPlus_Tick(object sender, EventArgs e)
        {          
            TimerPlus.Stop();

            if (currentType == TestType.Practice)
            {
                if (currentTrial == 2)
                    currentTrial = 0;
                else if (currentTrial == 1)
                    currentTrial = 0;
                else
                    currentTrial = 1;         
            }
            else
                RandomTrial();

            panelPlus.Hide();
            panelPlus.Dock = System.Windows.Forms.DockStyle.None;
            panelFraction.Show();
            panelFraction.Dock = System.Windows.Forms.DockStyle.Fill;

            labelFrac1.Font = new Font(labelFrac1.Font, FontStyle.Regular);
            labelFrac2.Font = new Font(labelFrac2.Font, FontStyle.Regular);

            Prime();
        }

        //랜덤 시행
        private void RandomTrial()
        {
            bool okFlag = false;
            while (!okFlag)
            {
                currentTrial = rnd.Next(0, 2);
                switch (currentTrial)
                {
                    case 0:
                        if (currentTrial == prevTrial)
                        {
                            if (isDoubleTestOK)
                            {
                                isDoubleTestOK = false;
                                isDoubleControlOK = true;
                            }
                            else
                                break;
                        }

                        if (countTest > 0)
                        {
                            prevTrial = currentTrial;
                            countTrial--;
                            countTest--;
                            okFlag = true;
                        }
                        break;

                    case 1:
                        if (currentTrial == prevTrial)
                        {
                            if (isDoubleControlOK)
                            {
                                isDoubleControlOK = false;
                                isDoubleTestOK = true;
                            }
                            else
                                break;
                        }

                        if (countControl > 0)
                        {
                            prevTrial = currentTrial;
                            countTrial--;
                            countControl--;
                            okFlag = true;
                        }
                        break;
                }
            }
        }
      
        private void Prime()
        {
            if (currentType == TestType.Practice)
            {
                if (currentFrac == -1)
                    currentFrac = 0;
                else
                    currentFrac++;
                countTest--;
            }
            else
            {
                while (currentFrac == prevPrimeFrac)
                    currentFrac = rnd.Next(0, 12);

                prevPrimeFrac = currentFrac;
            }

            if (currentTrial == 0)
            {
                labelFrac1.Text = PrimeQuiz1[currentFrac].FirstFraction;
                labelFrac2.Text = PrimeQuiz1[currentFrac].SecondFraction;
            }
            else
            {
                labelFrac1.Text = "#";
                labelFrac2.Text = "#";

                if (PrimeQuiz1[currentFrac].isFirstBigger)
                    labelFrac1.Font = new Font(labelFrac1.Font, FontStyle.Underline);
                else
                    labelFrac2.Font = new Font(labelFrac2.Font, FontStyle.Underline);
            }
            
            isTesting = true;
            swTest.Reset();
            swTest.Start();
        }

        private void Probe()
        {
            if (currentType == TestType.Practice)
            {
                countControl--;
                countTrial--;
            }
            else
            {
                while (currentFrac == prevProbeFrac)
                    currentFrac = rnd.Next(0, 12);

                prevProbeFrac = currentFrac;
            }

            labelFrac1.Text = ProbeQuiz1[currentFrac].FirstFraction;
            labelFrac2.Text = ProbeQuiz1[currentFrac].SecondFraction;

            isTesting = true;
            swTest.Reset();
            swTest.Start();
        }

        //분수 클릭
        //private void labelFrac1_Click(object sender, EventArgs e)
        //{
           
        //}

        //전체 초기화
        private void InitializeTest()
        {
            switch (currentType)
            {
                case TestType.Practice:
                    PrimeQuiz1.Add(new Quiz(1, 3, 7, 8, rnd));
                    PrimeQuiz1.Add(new Quiz(3, 4, 1, 4, rnd));
                    PrimeQuiz1.Add(new Quiz(1, 13, 2, 15, rnd));

                    ProbeQuiz1.Add(new Quiz(3, 4, 1, 4, rnd));
                    ProbeQuiz1.Add(new Quiz(3, 5, 4, 5, rnd));
                    ProbeQuiz1.Add(new Quiz(1, 13, 4, 13, rnd));
                    break;

                case TestType.Test1:                            //과제1 Prime 분수리스트 초기화
                    PrimeQuiz1.Add(new Quiz(1, 2, 2, 7, rnd));
                    PrimeQuiz1.Add(new Quiz(7, 8, 1, 9, rnd));
                    PrimeQuiz1.Add(new Quiz(1, 6, 7, 9, rnd));
                    PrimeQuiz1.Add(new Quiz(1, 3, 5, 8, rnd));
                    PrimeQuiz1.Add(new Quiz(3, 4, 8, 9, rnd));
                    PrimeQuiz1.Add(new Quiz(3, 7, 7, 8, rnd));
                    PrimeQuiz1.Add(new Quiz(2, 3, 1, 6, rnd));
                    PrimeQuiz1.Add(new Quiz(1, 5, 7, 8, rnd));
                    PrimeQuiz1.Add(new Quiz(6, 7, 2, 9, rnd));
                    PrimeQuiz1.Add(new Quiz(4, 5, 2, 8, rnd));
                    PrimeQuiz1.Add(new Quiz(2, 5, 7, 9, rnd));
                    PrimeQuiz1.Add(new Quiz(3, 8, 8, 9, rnd));
                                                                //과제1 Probe 분수리스트 초기화
                    ProbeQuiz1.Add(new Quiz(1, 3, 2, 3, rnd));
                    ProbeQuiz1.Add(new Quiz(1, 7, 6, 7, rnd));
                    ProbeQuiz1.Add(new Quiz(5, 8, 6, 8, rnd));
                    ProbeQuiz1.Add(new Quiz(2, 4, 3, 4, rnd));
                    ProbeQuiz1.Add(new Quiz(2, 7, 5, 7, rnd));
                    ProbeQuiz1.Add(new Quiz(1, 9, 6, 9, rnd));
                    ProbeQuiz1.Add(new Quiz(2, 6, 5, 6, rnd));
                    ProbeQuiz1.Add(new Quiz(1, 8, 6, 8, rnd));
                    ProbeQuiz1.Add(new Quiz(2, 9, 7, 9, rnd));
                    ProbeQuiz1.Add(new Quiz(4, 6, 5, 6, rnd));
                    ProbeQuiz1.Add(new Quiz(4, 8, 7, 8, rnd));
                    ProbeQuiz1.Add(new Quiz(5, 9, 8, 9, rnd));
                    break;

                case TestType.Test2:           
                    PrimeQuiz1.Add(new Quiz(1, 12, 9, 17, rnd));
                    PrimeQuiz1.Add(new Quiz(2, 14, 1, 19, rnd));
                    PrimeQuiz1.Add(new Quiz(1, 16, 9, 19, rnd));
                    PrimeQuiz1.Add(new Quiz(1, 13, 7, 18, rnd));
                    PrimeQuiz1.Add(new Quiz(7, 11, 2, 19, rnd));
                    PrimeQuiz1.Add(new Quiz(5, 17, 2, 18, rnd));
                    PrimeQuiz1.Add(new Quiz(2, 13, 1, 16, rnd));
                    PrimeQuiz1.Add(new Quiz(1, 15, 7, 11, rnd));
                    PrimeQuiz1.Add(new Quiz(7, 18, 2, 19, rnd));
                    PrimeQuiz1.Add(new Quiz(8, 13, 3, 18, rnd));
                    PrimeQuiz1.Add(new Quiz(2, 15, 8, 11, rnd));
                    PrimeQuiz1.Add(new Quiz(7, 18, 1, 15, rnd));

                    ProbeQuiz1.Add(new Quiz(1, 11, 2, 11, rnd));
                    ProbeQuiz1.Add(new Quiz(1, 17, 6, 17, rnd));
                    ProbeQuiz1.Add(new Quiz(5, 18, 6, 18, rnd));
                    ProbeQuiz1.Add(new Quiz(2, 14, 3, 14, rnd));
                    ProbeQuiz1.Add(new Quiz(2, 17, 5, 17, rnd));
                    ProbeQuiz1.Add(new Quiz(1, 19, 6, 19, rnd));
                    ProbeQuiz1.Add(new Quiz(2, 16, 5, 16, rnd));
                    ProbeQuiz1.Add(new Quiz(1, 18, 6, 18, rnd));
                    ProbeQuiz1.Add(new Quiz(2, 19, 7, 19, rnd));
                    ProbeQuiz1.Add(new Quiz(4, 16, 5, 16, rnd));
                    ProbeQuiz1.Add(new Quiz(4, 18, 7, 18, rnd));
                    ProbeQuiz1.Add(new Quiz(5, 19, 8, 19, rnd));
                    break;
            }
        }

        //"처음으로" 버튼 클릭
        private void btnHome_Click(object sender, EventArgs e)
        {
            panelResult.Hide();
            panelResult.Dock = System.Windows.Forms.DockStyle.None;
            panelReadyTest.Hide();
            panelReadyTest.Dock = System.Windows.Forms.DockStyle.None;
            panelMenu.Show();
            panelMenu.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        //참가자이름 제한
        private void tboxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Char.IsPunctuation(e.KeyChar) || Char.IsSymbol(e.KeyChar) || Char.IsSeparator(e.KeyChar)) && (e.KeyChar != 8))
            {
                e.Handled = true;
                MessageBox.Show("특수문자는 허용되지 않습니다.", "오류");
            }
        }

        /*분수라벨에서 마우스가 들어올때
        private void labelFrac1_MouseMove(object sender, MouseEventArgs e)
        {
            Label labelHover = (Label)sender;
            if (labelHover.Font.Underline)
                labelHover.Font = new Font(labelHover.Font, FontStyle.Bold | FontStyle.Underline);
            else
                labelHover.Font = new Font(labelHover.Font, FontStyle.Bold);
        }

        분수라벨에서 마우스가 나갈때
        private void labelFrac1_MouseLeave(object sender, EventArgs e)
        {
            Label labelHover = (Label)sender;
            if (labelHover.Font.Underline)
                labelHover.Font = new Font(labelHover.Font, FontStyle.Regular | FontStyle.Underline);
            else
                labelHover.Font = new Font(labelHover.Font, FontStyle.Regular);
        }*/

        //강제종료 메뉴 클릭
        private void MenuForceStop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("정말로 테스트를 종료하시겠습니까?", "테스트 종료", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                isTesting = false;
                panelFraction.Dock = System.Windows.Forms.DockStyle.None;
                panelFraction.Hide();
                panelPlus.Hide();
                panelReadyTest.Hide();
                panelMenu.Show();
                panelMenu.Dock = System.Windows.Forms.DockStyle.Fill;

                MenuHome.Enabled = true;
                MenuForceStop.Enabled = false;
                MenuShowResult.Enabled = true;

                PrimeQuiz1.Clear();
                ProbeQuiz1.Clear();
            }
        }

        //홈 메뉴 클릭
        private void MenuHome_Click(object sender, EventArgs e)
        {
            panelResult.Hide();
            panelResult.Dock = System.Windows.Forms.DockStyle.None;
            panelReadyTest.Hide();
            panelReadyTest.Dock = System.Windows.Forms.DockStyle.None;
            panelMenu.Show();
            panelMenu.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        //결과보기 메뉴 클릭
        private void MenuShowResult_Click(object sender, EventArgs e)
        {
            panelResult.Dock = System.Windows.Forms.DockStyle.Fill;
            panelResult.Show();
        }

        //이 프로그램에 대하여... 메뉴 클릭
        private void MenuInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Developed by Hohyeon Kim, 2017", "분수 억제 테스트");
        }

        //프로그램 닫을때
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("정말로 종료하시겠습니까?\r\r종료하시기전에 결과를 다른곳에 저장하시길 바랍니다.", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Dispose(true);
            else
            {
                e.Cancel = true;
                return;
            }   
        }

        //키 눌렸을때
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.A || e.KeyCode == Keys.L) && (isTesting == true))
            {
                swTest.Stop();
                isTesting = false;

                float swTime = swTest.ElapsedMilliseconds / 1000.0F;
                int primerw;
                int primert;
                int proberw;
                int probert;

                //Label LabelClicked = (Label)sender;

                labelFrac1.Font = new Font(labelFrac1.Font, FontStyle.Regular);
                labelFrac2.Font = new Font(labelFrac2.Font, FontStyle.Regular);

                if (currentTrial == 0) //TestTrial
                {
                    primerw = testprimerw;
                    primert = testprimert;
                    proberw = testproberw;
                    probert = testprobert;
                }
                else                    //ControlTrial
                {
                    primerw = controlprimerw;
                    primert = controlprimert;
                    proberw = controlproberw;
                    probert = controlprobert;
                }

                if (currentQuiz == 0)           //prime
                {
                    if (e.KeyCode == Keys.A)                 //LabelClicked.Name == "labelFrac1"
                    {
                        if (PrimeQuiz1[currentFrac].isFirstBigger)
                            result[primerw] = 1;
                        else
                            result[primerw] = 0;
                    }
                    else
                    {
                        if (PrimeQuiz1[currentFrac].isFirstBigger)
                            result[primerw] = 0;
                        else
                            result[primerw] = 1;
                    }
                    result[primert] = swTime;
                    currentQuiz++;          // prime -> probe
                    Probe();
                }
                else                             //probe
                {
                    if (e.KeyCode == Keys.A)                                       //LabelClicked.Name == "labelFrac1"
                    {
                        if (ProbeQuiz1[currentFrac].isFirstBigger)
                            result[proberw] = 1;
                        else
                            result[proberw] = 0;
                    }
                    else
                    {
                        if (ProbeQuiz1[currentFrac].isFirstBigger)
                            result[proberw] = 0;
                        else
                            result[proberw] = 1;
                    }
                    result[probert] = swTime;
                    currentQuiz--;          // probe -> prime
                    if (countTrial > 0)
                    {
                        if (currentTrial == 0)
                        {
                            testprimerw++;
                            testprimert++;
                            testproberw++;
                            testprobert++;
                        }
                        else
                        {
                            controlprimerw++;
                            controlprimert++;
                            controlproberw++;
                            controlprobert++;
                        }
                        ShowPlus();
                    }
                    else
                    {
                        isTesting = false;

                        MessageBox.Show("테스트가 끝났습니다.", "알림");

                        panelFraction.Dock = System.Windows.Forms.DockStyle.None;
                        panelFraction.Hide();
                        panelPlus.Hide();
                        panelReadyTest.Hide();
                        panelMenu.Show();
                        panelMenu.Dock = System.Windows.Forms.DockStyle.Fill;

                        MenuHome.Enabled = true;
                        MenuForceStop.Enabled = false;
                        MenuShowResult.Enabled = true;

                        PrimeQuiz1.Clear();
                        ProbeQuiz1.Clear();

                        if (currentType != TestType.Practice)
                        {
                            dataResult.Rows.Add(tboxName.Text);
                            for (int i = 0; i < 96; i++)
                            {
                                dataResult[i + 1, countParticipant].Value = result[i];
                            }
                            countParticipant++;
                        } 
                    }
                }
            }
        }

    }
}
