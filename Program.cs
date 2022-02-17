using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncBreakfast
{
    class Program
    {
        static async Task Main(string[] args)
        {                
            await test1();
            Console.WriteLine(">>>>>> end Test <<<<<<<");
            Console.WriteLine("######################################################################################################");
            Console.WriteLine("Press any key");
            Console.WriteLine(Environment.NewLine);
            Console.ReadKey();
            
            await test2();
            Console.WriteLine(">>>>>> end Test <<<<<<<");
            Console.WriteLine("######################################################################################################");
            Console.WriteLine("Press any key");
            Console.WriteLine(Environment.NewLine);
            Console.ReadKey();
            
            await testBreakfest();
            Console.WriteLine(">>>>>> end Test <<<<<<<");
        }

        static async Task<Toast> MakeToastWithButterAndJamAsync(int number)
        {
            var toast = await ToastBreadAsync(number);
            ApplyButter(toast);
            ApplyJam(toast);

            return toast;
        }

        private static Juice PourOJ()
        {
            Console.WriteLine("Pouring orange juice");
            return new Juice();
        }

        private static void ApplyJam(Toast toast) =>
            Console.WriteLine("Putting jam on the toast");

        private static void ApplyButter(Toast toast) =>
            Console.WriteLine("Putting butter on the toast");

        private static async Task<Toast> ToastBreadAsync(int slices)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("Putting a slice of bread in the toaster");
            }
            Console.WriteLine("Start toasting...");
            await Task.Delay(3000);
            Console.WriteLine("Remove toast from toaster");

            return new Toast();
        }

        private static async Task<Bacon> FryBaconAsync(int slices)
        {
            Console.WriteLine($"putting {slices} slices of bacon in the pan");
            Console.WriteLine("cooking first side of bacon...");
            await Task.Delay(3000);
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("flipping a slice of bacon");
            }
            Console.WriteLine("cooking the second side of bacon...");
            await Task.Delay(3000);
            Console.WriteLine("Put bacon on plate");

            return new Bacon();
        }

        private static async Task<Egg> FryEggsAsync(int howMany)
        {
            Console.WriteLine("Warming the egg pan...");
            await Task.Delay(3000);
            Console.WriteLine($"cracking {howMany} eggs");
            Console.WriteLine("cooking the eggs ...");
            await Task.Delay(3000);
            Console.WriteLine("Put eggs on plate");
            
            return new Egg();
        }

        private static Coffee PourCoffee()
        {
            Console.WriteLine("Pouring coffee");
            return new Coffee();
        }    

        private static async Task testBreakfest(){
                        Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready");

            var eggsTask = FryEggsAsync(2);
            var baconTask = FryBaconAsync(3);
            var toastTask = MakeToastWithButterAndJamAsync(2);

            var breakfastTasks = new List<Task> { eggsTask, baconTask, toastTask };
            while (breakfastTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(breakfastTasks);
                if (finishedTask == eggsTask)
                {
                    Console.WriteLine("eggs are ready");
                }
                else if (finishedTask == baconTask)
                {
                    Console.WriteLine("bacon is ready");
                }
                else if (finishedTask == toastTask)
                {
                    Console.WriteLine("toast is ready");
                }
                breakfastTasks.Remove(finishedTask);
            }

            Juice oj = PourOJ();
            Console.WriteLine("oj is ready");
            Console.WriteLine("Breakfast is ready!");
        }
        private static async Task test1(){
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;

            var task = Task.Run(() =>
            {
                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                bool moreToDo = true;
                while (moreToDo)
                {
                    Console.WriteLine("Do more running");
                    Task.Delay(10000);
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                       Console.WriteLine("request IsCancellationRequested");
                       // Clean up here, then...
                       ct.ThrowIfCancellationRequested();
                    }
                }                
                
            }, tokenSource2.Token);

            //After key press will stop with ct.ThrowIfCancellationRequested();
            Console.ReadKey();
            tokenSource2.Cancel();

            // Just continue on this thread, or await with try-catch:
            try
            {
                Console.WriteLine("Waiting task");
                await task;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
            finally
            {
                tokenSource2.Dispose();
            }
        }
        private static async Task test2(){
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;

            var task = Task.Run(() =>
            {
                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                int count = 0;
                while (true)
                {
                    count++;
                    Console.WriteLine("Do more running");
                    Task.Delay(1000);
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (count > 150)
                    {
                        Console.WriteLine("request IsCancellationRequested");
                        throw new ApplicationException("Manually cancel");
                    }
                }                
                
            }, tokenSource2.Token).ContinueWith( t => { 
                                                Console.WriteLine("ContinueWith exception IsCancellationRequested");
                                                Console.WriteLine("{0}: {1}",
                                                t.Exception.InnerException.GetType().Name,
                                                t.Exception.InnerException.Message);
                            }, TaskContinuationOptions.OnlyOnFaulted);            
            
            // Just continue on this thread, or await with try-catch:
            try
            {
                Console.WriteLine("Waiting task2");
                await task;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{nameof(e)} thrown with message: {e.Message}");
            }
            finally
            {
                tokenSource2.Dispose();
            }
        }        
    }
}