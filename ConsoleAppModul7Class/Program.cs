using System;
//using System.Collections.Generic;

namespace ConsoleAppModul7Class
{
    abstract class Delivery
    {
        //protected string addrDeliv; // адрес доставки

        // адрес доставки
        public virtual string AddrDeliv {get;set;}
        // дата доставки
        public virtual DateTime DateDeliv {get;set;}

    }

    class HomeDelivery : Delivery
    {//доставка клиенту на дом посредством курьера
        public static decimal MaxWeightOrder =20;     // статик max kg вес заказа для доставки курьером 
        private DateTime DateHomeDelivery; // согласованная с покупателем дата доставки ему на дом 
        
        public HomeDelivery() { DateHomeDelivery = DateTime.Now.AddDays(3); } // если конструктор путой то дата доставки на 3 день.
        public HomeDelivery(DateTime dateHomeDelivery) { DateHomeDelivery = dateHomeDelivery; } // переопределяем консмтруктор, для ввода даты доставки
        //Свойства Курьер                             
        public Courier Courier { get; set; }//Курьер выполняющий доставку
        //Свойства Доставки на дом                              
        public override DateTime DateDeliv
        {
            get
            {
                return DateHomeDelivery;
            }
            set
            {
                DateHomeDelivery = value;
            }
        }
        //Свойства адрес доставки                              
        //public override string AddrDeliv
        //{
        //    get
        //    {
        //        return addrDeliv;
        //    }
        //    set
        //    {
        //        addrDeliv = value;
        //    }
        //}

    }
    class PickPointDelivery : Delivery
    {//доставка в пункт выдачи (например ТЦ)
        public static int StorageDay=5;      //допустимое кол-во дней хранения в пункте выдачи
        private DateTime DateBegin; // дата прибытия товара в пункт выдачи
        private DateTime DateEnd;   // дата окончания хранения товара в пункте выдачи

        public PickPointDelivery() { DateBegin = DateTime.Now.AddDays(1); DateEnd = DateTime.Now.AddDays(StorageDay); }
        public PickPointDelivery(DateTime dateBegin) { DateBegin = dateBegin; DateEnd = dateBegin.AddDays(StorageDay); }
        public PickPointDelivery(DateTime dateBegin, DateTime dateEnd) { DateBegin = dateBegin; DateEnd = dateEnd; }


        public void CheckDateEnd()
        {
            if (DateTime.Now > DateEnd)
            {
                Console.WriteLine("Срок хранения исчерпан, товар вернулся на склад.");
            }
            else
            {
                Console.WriteLine("Товар Вас ожидает {0} д. до {1} ", (DateEnd- DateTime.Now).Days, DateEnd );
            }
        }

            //дата доставки в пунк выдачи                         
         public override DateTime DateDeliv
        {     
            get
            { 
                return DateBegin;
            }
            set
            {
                if (value > DateTime.Now.AddDays(1)) // мы можем доставлять только на следующий день
                {
                    DateBegin = value;
                    DateEnd = value.AddDays(StorageDay);
                }
                else
                {
                    Console.WriteLine("Mы можем доставлять только на следующий день");
                }

            }
        }
       
    }
    class SkladDelivery : Delivery
    {//самовывоз со склада 
        public static int StorageDay = 6;      //допустимое кол-во дней нахождения на брони
        private DateTime DateEnd;   // дата снятия с брони (

        public SkladDelivery() { DateEnd = DateTime.Now.AddDays(StorageDay); }


        //дата постанови на бронь                      
        public override DateTime DateDeliv
        {
            get
            {
                return DateEnd;
            }
            set
            {
                if (value >= DateTime.Now) // мы можем выдать товар сразу
                {
                    DateEnd = value.AddDays(StorageDay);
                }
                else
                {
                    Console.WriteLine("Некорректная дата, должна быть больше текущей");
                }

            }
        }
        
    }
    class Order<TDelivery, TListOrderTov> 
        where TDelivery : Delivery
    {
        public TDelivery Delivery; // Вариант доставки

        public TListOrderTov[] ListOrderTov; // Список товаров (товар, цена, количество)

        public Customer сustomer; // Клиент
        
        public int Number; // Номер заказа

        public string Description; // Описание, коментарий к заказу

        public decimal Summa { get; set; } //общая сумма заказа

        public void DisplayAddress()
        {
            Console.WriteLine("Адрес доставки Заказа: {0}", Delivery.AddrDeliv);
        }

        public decimal CalcSummaOrder()
        {   //общая сумма заказа
            decimal Summa = 0;
            //foreach (TListOrderTov ot in ListOrderTov)
            //{
            //    summa += ot.price * ot.Count; 
            //}
            return Summa;
        }
        public void ReservOrder() 
        {
            Console.WriteLine("Формирование заказа и постановка товаров на бронь...");
            Console.WriteLine("Заказ создан успешно.");

            if (сustomer.TypePay == Typepay.Prepayment)
            {
                Console.WriteLine("Оплатите заказ. Сумма заказа {0}", Summa);
            }
            else 
            {
                Console.WriteLine("Формирование счета для оплаты при получении товара");
            }
           
            if ("Флаг оплаты"=="Отказано") // Тут проверка факта оплаты
            {
                Console.WriteLine("Снятие бронирования, т.к. товар не оплачен");
            }
        }


    }
    //----------Добавлены собственные классы---------------------------
    abstract class Product
    { //товар
       public string ntov; // наименование товара
       public decimal wheight; // вес товара
       public void GetInfo()
       {
            Console.WriteLine("Наименование товара: {0}  Вес: {1}", ntov, wheight);
       }
    }
    class OrderTov : Product
    { //товары на складе - это товары с ценой и остатками
       public decimal price; //цена товара, нельзя менять после создания
       public int Count { get; set; }// количествоо
       public new void GetInfo() //Сокрытие базового метода
        {
            base.GetInfo(); 
            // к базовому методу дополним информцию
            Console.WriteLine($"Цена: {price}  Количество: {Count}");
        }
        public OrderTov() {}
        public OrderTov(string Ntov, decimal Price) { ntov = Ntov; price= Price; Count = 1; wheight = 1; } // Сразу указываем, что одна штука
        public OrderTov(string Ntov, int count) { ntov = Ntov; price = 100; Count = count; wheight = 1; }
        public OrderTov(string Ntov, decimal Price, int count) { ntov = Ntov; price = Price; Count = count; wheight = 1; }

    }
    abstract class Person
    {
        public string Name { get; set; }// имя
        public string Telephone { get; set; }// телефон
        public void GetInfo()
            {
                Console.WriteLine("Имя: {0}  Телефон: {1}", Name, Telephone);
            }
       }
    public enum Typepay : int { PaymentOnDelivery, Prepayment }; //способ оплаты заказа после получения или предоплата
    // Покупатель товаров
    class Customer : Person
    {
        public Typepay TypePay { get; set; }//способ оплаты заказа после получения или предоплата
        public Customer() { TypePay=Typepay.PaymentOnDelivery; }
    }
    // Курьер доставки
    class Courier : Person
    {
        public static decimal MaxWeightDelivery=20; //макс вес которорый может доставить курьер
    }
    //---------------------------------------------------------------------

        class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            int cnt;
            string name;
            decimal sumWheight = 0;
            decimal summa = 0;
            Delivery curDelivery; 

            OrderTov[] listOrderTov = new OrderTov[3];

            Console.WriteLine("Сделайте заказ из 3х товаров.");
                        
            do
            { 
                Console.Write("Введите наименование товара:");
                name = Console.ReadLine();
                
                Console.Write("Введите количество цифрой:");
                cnt = Convert.ToInt32(Console.ReadLine());
                
                listOrderTov[i] = new OrderTov(name, cnt);
                i++;

            } while (cnt > 0 &&  i < 3);

            Console.WriteLine("Ваши товары:");
            
            foreach (OrderTov ot in listOrderTov)
            {
                ot.GetInfo();
                sumWheight = sumWheight + ot.wheight; //общий вес заказа
                summa += ot.price * ot.Count        ; //общая сумма заказа
            }

            Console.WriteLine($"Общий вес заказа {sumWheight}, Общая сумма заказа {summa}");
            
            
            if (Courier.MaxWeightDelivery < sumWheight)
            {
                Console.WriteLine($"Доставка курьером невозможна, т.к. общий вес превысил максимально возможный {Courier.MaxWeightDelivery }");
               
                Console.Write("Выберите доставку: введите 0 - если 'до точки выдачи' иначе 'самовывоз со склада' :");
               
            }
            else
            {
                Console.Write("Выберите доставку: введите 0 - если 'До точки выдачи', 1 - 'Курьер до дома' иначе 'Cамовывоз со склада' :");
                
            }
            i = Convert.ToInt32(Console.ReadLine());
            if (i == 0)
            { 
                curDelivery = new PickPointDelivery();
            }
            else if (i== 1 || Courier.MaxWeightDelivery < sumWheight)
            {
                curDelivery = new HomeDelivery();
            }
            else
            {
                curDelivery = new SkladDelivery();   
            }

            Console.Write("Укажите адрес доставки:");
            curDelivery.AddrDeliv = Console.ReadLine();
            
            Order<Delivery, Product> curOrder = new Order<Delivery, Product> 
            { Delivery = curDelivery, ListOrderTov = listOrderTov, Summa=summa, Number = 1
            };
            
            curOrder.сustomer = new Customer { TypePay = Typepay.Prepayment };

            curOrder.DisplayAddress();
            curOrder.ReservOrder();
        }
    }
}
