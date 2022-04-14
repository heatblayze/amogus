using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace amogus
{
    internal class CrewmateController
    {
        const string resourcePrefix = "pack://application:,,,/Resources/amogus_";

        Window targetWindow;
        Image crewmate;
        bool flipped;

        double targetX, targetY;

        double currentX, currentY;
        double velocityX, velocityY;
        double maxVelocity = 300;
        double acceleration = 300;
        double deceleration = 10;
        double positionEpsilon = 10;
        double velocityEpsilon = 1;
        double delta = 0.01666;

        string idleFrame = "idle_right";
        string[] walkFrames =
        {
            "idle_right",
            "walk1",
            "walk2",
            "walk1",
            "idle_right",
            "walk3",
            "walk4",
            "walk3",
            "idle_right",
        };

        int currentFrame = 0;

        public CrewmateController(Image crewmate, Window targetWindow)
        {
            this.crewmate = crewmate;
            this.targetWindow = targetWindow;            

            var x = RandomUtil.RandomDouble(0, targetWindow.ActualWidth - crewmate.ActualWidth);
            var y = RandomUtil.RandomDouble(0, targetWindow.ActualHeight - crewmate.ActualHeight);

            currentX = x;
            currentY = y;

            crewmate.Margin = new Thickness(x, y, 0, 0);

            flipped = RandomUtil.random.Next(0, 2) == 1;
            SetFlipCrewmate();

            Task.Run(ChooseMotion);
        }

        void SetFlipCrewmate()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ScaleTransform flipTrans = new ScaleTransform();
                flipTrans.ScaleX = flipped ? -1 : 1;                
                crewmate.RenderTransform = flipTrans;
            });
        }

        void SetWalkFrame()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                crewmate.Source = new BitmapImage(new Uri(resourcePrefix + walkFrames[currentFrame] + ".png"));

                ++currentFrame;
                if (currentFrame >= walkFrames.Length - 1)
                {
                    currentFrame = 0;
                }
            });
        }

        void SetIdle()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                crewmate.Source = new BitmapImage(new Uri(resourcePrefix + idleFrame + ".png"));
                currentFrame = 0;
            });
        }

        void SetCrewmatePosition()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                crewmate.Margin = new Thickness(currentX, currentY, 0, 0);
            });
        }

        void GetRandomPoint()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                targetX = RandomUtil.RandomDouble(0, targetWindow.ActualWidth - crewmate.ActualWidth);
                targetY = RandomUtil.RandomDouble(0, targetWindow.ActualHeight - crewmate.ActualHeight);
            });
        }

        async Task MoveToPoint(double x, double y)
        {
            bool stop = false;
            do
            {
                if (!stop)
                {
                    if (Math.Abs(x - currentX) > positionEpsilon || Math.Abs(y - currentY) > positionEpsilon)
                    {
                        var diffX = x - currentX;
                        var diffY = y - currentY;

                        var len = Math.Sqrt(Math.Pow(Math.Abs(diffX), 2) + Math.Pow(Math.Abs(diffY), 2));

                        velocityX += diffX / len * delta * acceleration;
                        velocityY += diffY / len * delta * acceleration;
                    }
                    else
                    {
                        stop = true;
                    }
                }

                // Adjust the velocity
                {
                    var len = Math.Sqrt(Math.Pow(Math.Abs(velocityX), 2) + Math.Pow(Math.Abs(velocityY), 2));

                    if (len > maxVelocity)
                    {
                        // Clamp velocity
                        velocityX = velocityX * maxVelocity / len;
                        velocityY = velocityY * maxVelocity / len;

                        len = maxVelocity;
                    }

                    if (stop)
                    {
                        velocityX = velocityX * (len - (len / deceleration)) / len;
                        velocityY = velocityY * (len - (len / deceleration)) / len;
                    }
                }

                currentX += velocityX * delta;
                currentY += velocityY * delta;

                SetCrewmatePosition();
                await Task.Delay((int)(delta * 1000));
            } while (Math.Abs(velocityX) > velocityEpsilon && Math.Abs(velocityY) > velocityEpsilon);

            velocityX = 0;
            velocityY = 0;
        }

        async Task Animate()
        {
            do
            {
                // Always animate at ~30fps
                await Task.Delay(100);
                SetWalkFrame();
            } while (Math.Abs(velocityX) > velocityEpsilon && Math.Abs(velocityY) > velocityEpsilon);
        }

        async Task DoWalk()
        {
            GetRandomPoint();
            await Task.Delay(30);

            flipped = currentX > targetX;
            SetFlipCrewmate();

            Task.Run(Animate);
            await MoveToPoint(targetX, targetY);
        }

        async Task DoKnife()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                crewmate.Source = new BitmapImage(new Uri(resourcePrefix + "knife.png"));
                currentFrame = 0;
            });

            var sri = Application.GetResourceStream(new Uri(resourcePrefix + "reveal.wav"));
            if ((sri != null))
            {
                using (var s = sri.Stream)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(s);
                    player.Load();
                    player.Play();
                }
            }

            await Task.Delay(2500);
        }

        async Task Flip()
        {
            flipped = !flipped;
            SetFlipCrewmate();
            await Task.Delay(1000);
        }

        async Task ChooseMotion()
        {
            await Task.Delay(RandomUtil.random.Next(1000, 10000));

            int rand = RandomUtil.random.Next(0, 100);
            if (rand >= 95)
            {
                await DoKnife();
            }
            else if(rand >= 90)
            {
                await Flip();
            }
            else
            {
                await DoWalk();
            }

            SetIdle();
            await ChooseMotion();
        }        
    }
}
