using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int maximumPlayerMoney = 1000;
            int minimumPlayerMoney = 500;
            int maximumQuantityProduct = 50;
            int minimumQuantityProduct = 10;

            Random random = new Random();
            Salesman salesman = new Salesman();
            Player player = new Player(random.Next(minimumPlayerMoney, maximumPlayerMoney));
            Stack stack;
            Product product;


            salesman.AddProduct(new Stack(new Product("Хлеб", 40), random.Next(minimumQuantityProduct, maximumQuantityProduct)));
            salesman.AddProduct(new Stack(new Product("Бутылка воды", 20), random.Next(minimumQuantityProduct, maximumQuantityProduct)));
            salesman.AddProduct(new Stack(new Product("Колбаса", 250), random.Next(minimumQuantityProduct, maximumQuantityProduct)));
            salesman.AddProduct(new Stack(new Product("Сыр", 200), random.Next(minimumQuantityProduct, maximumQuantityProduct)));
            salesman.AddProduct(new Stack(new Product("Чипсы", 70), random.Next(minimumQuantityProduct, maximumQuantityProduct)));
            salesman.AddProduct(new Stack(new Product("Сухарики", 30), random.Next(minimumQuantityProduct, maximumQuantityProduct)));
            salesman.AddProduct(new Stack(new Product("Дошик", 20), random.Next(minimumQuantityProduct, maximumQuantityProduct)));

            bool isTrade = true;

            while (isTrade)
            {
                Console.WriteLine($"{(int)MenuCommand.BuyProduct}. {MenuCommand.BuyProduct}" +
                                  $"\n{(int)MenuCommand.MyProduct}. {MenuCommand.MyProduct}" +
                                  $"\n{(int)MenuCommand.ExitShop}. {MenuCommand.ExitShop}");
                int userInput = GetNumber(Console.ReadLine());
                Console.Clear();

                switch (userInput)
                {
                    case (int)MenuCommand.BuyProduct:
                        salesman.SellProduct(player);
                        break;
                    case (int)MenuCommand.MyProduct:
                        player.ShowListOfProducts();
                        break;
                    case (int)MenuCommand.ExitShop:
                        isTrade = false;
                        break;
                }

                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine("Выход ...");
        }

        public static int GetNumber(string numberText)
        {
            int number;

            while (int.TryParse(numberText, out number) == false)
            {
                Console.Write("Повторите попытку: ");
                numberText = Console.ReadLine();
            }

            return number;
        }
    }

    enum MenuCommand
    {
        BuyProduct = 1,
        MyProduct,
        ExitShop
    }

    abstract class Member
    {
        protected List<Stack> StackProducts = new List<Stack>();

        abstract public void ShowListOfProducts();
        
        public void AddProduct(Stack products)
        {
            StackProducts.Add(products);
        }
    }

    class Salesman : Member
    {
        private Stack _stackSellProduct;

        public int QuantitySellProduct { get; private set; }
        public Stack StackSellProduct { get { return _stackSellProduct; } }

        public override void ShowListOfProducts()
        {
            for (int i = 0; i < StackProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Продукт - {StackProducts[i].Product.Name} | Цена - {StackProducts[i].Product.Price} | В наличии - {StackProducts[i].Quantity}");
            }
        }

        public void SellProduct(Player player)
        {
            Console.SetCursorPosition(0, 20);
            Console.WriteLine("Ваши деньги: " + player.Money);
            Console.SetCursorPosition(0, 0);

            ShowListOfProducts();
            Console.Write("\nВыберите товар, который хотите приобрести: ");
            int numberProduct = Program.GetNumber(Console.ReadLine()) - 1;
            Console.Write("\nВведите кол-во товара, который хотите приобрести: ");
            QuantitySellProduct = Program.GetNumber(Console.ReadLine());

            if (TrySellProduct(numberProduct, out _stackSellProduct))
            {
                player.BuyProduct(this);
            }
            else
            {
                Console.WriteLine("Не удалось найти товар. Покупка отменена");
            }
        }

        private bool TrySellProduct(int index, out Stack product)
        {
            product = null;

            if (index >= StackProducts.Count || index < 0)
            {
                return false;
            }
            else if (QuantitySellProduct <= StackProducts[index].Quantity)
            {
                product = StackProducts[index];
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class Player : Member
    {
        public int Money { get; private set; }

        public Player(int money)
        {
            Money = money;
        }

        public override void ShowListOfProducts()
        {
            Console.WriteLine("Ваши приобретенные товары:\n");

            if (StackProducts.Count > 0)
            {
                for (int i = 0; i < StackProducts.Count; i++)
                {
                    Console.WriteLine($"Продукт - {StackProducts[i].Product.Name} |  Куплено - {StackProducts[i].Quantity}");
                }
            }
            else
            {
                Console.WriteLine("Вы ничего не покупали");
            }

            Console.WriteLine("\n\n\nОставшиеся деньги: " + Money);
        }

        public void BuyProduct(Salesman salesman)
        {
            int sum = salesman.StackSellProduct.Product.Price * salesman.QuantitySellProduct;

            if (CheckSolvency(sum))
            {
                Money -= sum;
                salesman.StackSellProduct.ReduceQuantity(salesman);
                if (CheckSameProduct(salesman.StackSellProduct))
                {
                    AddSameProduct(salesman);
                }
                else
                {
                    AddProduct(new Stack(salesman.StackSellProduct.Product, salesman.QuantitySellProduct));
                }
                Console.WriteLine("Покупка прошла успешно!");
            }
            else
            {
                Console.WriteLine("У вас не хватило денег. Покупка отменена");
            }
        }

        private void AddSameProduct(Salesman salesman)
        {
            for (int i = 0; i < StackProducts.Count; i++)
            {
                if (StackProducts[i].Product.Name == salesman.StackSellProduct.Product.Name)
                {
                    StackProducts[i].IncreaseQuantity(salesman);
                }
            }
        }

        private bool CheckSolvency(int sum)
        {
            return sum <= Money;
        }

        private bool CheckSameProduct(Stack product)
        {
            for (int i = 0; i < StackProducts.Count; i++)
            {
                if (StackProducts[i].Product.Name == product.Product.Name)
                {
                    return true;
                }
            }

            return false;
        }
    }

    class Stack
    {
        public Product Product { get; private set; }
        public int Quantity { get; private set; }

        public Stack(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public void IncreaseQuantity(Salesman salesman)
        {
            Quantity += salesman.QuantitySellProduct;
        }

        public void ReduceQuantity(Salesman salesman)
        {
            if (CheckQuantity(salesman.QuantitySellProduct))
            {
                Quantity -= salesman.QuantitySellProduct;
            }
            else
            {
                Console.WriteLine("Товар закончился :(");
            }
        }

        private bool CheckQuantity(int quantity)
        {
            return quantity <= Quantity;
        }
    }

    class Product
    {
        public string Name { get; private set; }
        public int Price { get; private set; }

        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }
    }
}