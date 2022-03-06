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

            salesman.AddProduct(new Product("Хлеб", 40, random.Next(10, 50)));
            salesman.AddProduct(new Product("Бутылка воды", 20, random.Next(10, 50)));
            salesman.AddProduct(new Product("Колбаса", 250, random.Next(10, 50)));
            salesman.AddProduct(new Product("Сыр", 200, random.Next(10, 50)));
            salesman.AddProduct(new Product("Чипсы", 70, random.Next(10, 50)));
            salesman.AddProduct(new Product("Сухарики", 30, random.Next(10, 50)));
            salesman.AddProduct(new Product("Дошик", 20, random.Next(10, 50)));

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

    class Salesman
    {
        public int QuantitySellProduct { get; private set; }
        private List<Product> _products = new List<Product>();

        public void SellProduct(Player player)
        {
            Console.SetCursorPosition(0, 20);
            Console.WriteLine("Ваши деньги: " + player.Money);
            Console.SetCursorPosition(0, 0);

            Product sellProduct;

            ShowListOfProducts();
            Console.Write("\nВыберите товар, который хотите приобрести: ");
            int numberProduct = Program.GetNumber(Console.ReadLine()) - 1;
            Console.Write("\nВведите кол-во товара, который хотите приобрести: ");
            QuantitySellProduct = Program.GetNumber(Console.ReadLine());

            if (TrySellProduct(numberProduct, QuantitySellProduct, out sellProduct))
            {
                player.BuyProduct(sellProduct, QuantitySellProduct);
                sellProduct.ReduceQuantity(this);
            }
            else
            {
                Console.WriteLine("Не удалось найти товар. Покупка отменена");
            }
            
            QuantitySellProduct = 0;
        }

        public void AddProduct(Product product)
        {
            _products.Add(product);
        }

        public void ShowListOfProducts()
        {
            for (int i = 0; i < _products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Продукт - {_products[i].Name} | Цена - {_products[i].Price} | В наличии - {_products[i].Quantity}");
            }
        }

        private bool TrySellProduct(int index, int quantity, out Product product)
        {
            product = null;

            if (quantity <= _products[index].Quantity)
            {
                product = _products[index];
                return true;
            }

            return false;
        }
    }

    class Product
    {
        public string Name { get; private set; }
        public int Price { get; private set; }
        public int Quantity { get; private set; }

        public Product(string name, int price, int quantity)
        {
            Name = name;
            Price = price;
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
            if (quantity <= Quantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class Player
    {
        private List<Product> _products = new List<Product>();
        public int Money { get; private set; }

        public Player(int money)
        {
            Money = money;
        }

        public void ShowListOfProducts()
        {
            Console.WriteLine("Ваши приобретенные товары:\n");

            if (_products.Count > 0)
            {
                for (int i = 0; i < _products.Count; i++)
                {
                    Console.WriteLine($"Продукт - {_products[i].Name} |  Куплено - {_products[i].Quantity}");
                }
            }
            else
            {
                Console.WriteLine("Вы ничего не покупали");
            }

            Console.WriteLine("\n\n\nОставшиеся деньги: " + Money);
        }

        public void BuyProduct(Product product, int quantity)
        {
            if (CheckSolvency(product, quantity))
            {
                Money -= product.Price * quantity;
                AddProduct(new Product(product.Name, product.Price, quantity));
                Console.WriteLine("Покупка прошла успешно!");
            }
            else
            {
                Console.WriteLine("У вас не хватило денег. Покупка отменена");
            }
        }

        private void AddProduct(Product product)
        {
            _products.Add(product);
        }

        private bool CheckSolvency(Product product, int quantity)
        {
            if (product.Price * quantity <= Money)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}