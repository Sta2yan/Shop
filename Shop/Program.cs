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
            Random random = new Random();
            Salesman salesman = new Salesman();
            Player player = new Player(random.Next(500, 1000));

            salesman.AddProduct(new Stack(new Product("Хлеб", 40), random.Next(10, 50)));
            salesman.AddProduct(new Stack(new Product("Бутылка воды", 20), random.Next(10, 50)));
            salesman.AddProduct(new Stack(new Product("Колбаса", 250), random.Next(10, 50)));
            salesman.AddProduct(new Stack(new Product("Сыр", 200), random.Next(10, 50)));
            salesman.AddProduct(new Stack(new Product("Чипсы", 70), random.Next(10, 50)));
            salesman.AddProduct(new Stack(new Product("Сухарики", 30), random.Next(10, 50)));
            salesman.AddProduct(new Stack(new Product("Дошик", 20), random.Next(10, 50)));

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
        protected List<Stack> _stackProducts = new List<Stack>();

        abstract public void ShowListOfProducts();

        abstract public void AddProduct(Stack products);
    }

    class Salesman : Member
    {
        public int QuantitySellProduct { get; private set; }

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

            Stack product;

            if (TrySellProduct(numberProduct, out product))
            {
                player.BuyProduct(product, QuantitySellProduct);
                product.ReduceQuantity(this);
            }
            else
            {
                Console.WriteLine("Не удалось найти товар. Покупка отменена");
            }
            
            QuantitySellProduct = 0;
        }

        public override void AddProduct(Stack stackProducts)
        {
            _stackProducts.Add(stackProducts);
        }

        public override void ShowListOfProducts()
        {
            for (int i = 0; i < _stackProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Продукт - {_stackProducts[i].Product.Name} | Цена - {_stackProducts[i].Product.Price} | В наличии - {_stackProducts[i].Quantity}");
            }
        }

        private bool TrySellProduct(int index, out Stack product)
        {
            product = null;

            if (index >= _stackProducts.Count || index < 0)
            {
                return false;
            }
            else if (QuantitySellProduct <= _stackProducts[index].Quantity)
            {
                product = _stackProducts[index];
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

            if (_stackProducts.Count > 0)
            {
                for (int i = 0; i < _stackProducts.Count; i++)
                {
                    Console.WriteLine($"Продукт - {_stackProducts[i].Product.Name} |  Куплено - {_stackProducts[i].Quantity}");
                }
            }
            else
            {
                Console.WriteLine("Вы ничего не покупали");
            }

            Console.WriteLine("\n\n\nОставшиеся деньги: " + Money);
        }

        public void BuyProduct(Stack product, int quantity)
        {
            int sum = product.Product.Price * quantity;

            if (CheckSolvency(sum))
            {
                Money -= sum;
                AddProduct(new Stack(product.Product, quantity));
                Console.WriteLine("Покупка прошла успешно!");
            }
            else
            {
                Console.WriteLine("У вас не хватило денег. Покупка отменена");
            }
        }

        public override void AddProduct(Stack product)
        {
            _stackProducts.Add(product);
        }

        private bool CheckSolvency(int sum)
        {
            return sum <= Money;
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