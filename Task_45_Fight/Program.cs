using System;
using System.Collections.Generic;

namespace Task_45_Fight
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();
            arena.StartGame();
        }
    }

    class Fighter
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int Attack { get; protected set; }
        public int CurrentDamage { get; protected set; }
        public int Armor { get; protected set; }
        public int AttackSpeed { get; protected set; }

        public Fighter(string name, int health, int attack, int armor, int attackSpeed)
        {
            Name = name;
            Health = health;
            Attack = attack;
            Armor = armor;
            AttackSpeed = attackSpeed;
        }
        
        public virtual void TakeDamage(int attack)
        {
            if (Armor >= attack)
            {
                Armor -= attack;
            }
            else
            {
                Health -= attack - Armor;
                Armor = 0;
            }
        }

        public virtual int ReturnDamage()
        {
            if (Health > 0)
            {
                OutputColorText($"{Name} наносит ", " урона", CurrentDamage.ToString());
                return CurrentDamage;
            }
            else
            {
                return CurrentDamage = 0;
            }
        }

        public void ShowStats()
        {
            OutputColorText($"{Name}: ", $" HP, {Armor} ARM", "", Health.ToString());
        }

        public void OutputColorText(string textLeft, string textRight, string redText = "", string greenText = "")
        {
            Console.Write($"{textLeft}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(redText);
            Console.Write(greenText);
            Console.ResetColor();
            Console.WriteLine($" {textRight}");
        }

        public bool RandomlyApplyAction(int percentProbabilityAction)
        {
            int lowLimit = 0;
            int maxLimit = 100;
            Random Random = new Random();
            int randomNumber = Random.Next(lowLimit, maxLimit);
            return randomNumber <= percentProbabilityAction;
        }
    }

    class Heavy : Fighter
    {
        public Heavy() : base("Тяжёлый", 200, 40, 0, 1) { }

        public override int ReturnDamage()
        {
            CurrentDamage = 0;

            for (int i = 0; i < AttackSpeed; i++)
            {
                CurrentDamage += Attack + StrongHit() + CriticalHit();
            }

            return base.ReturnDamage();
        }

        private int StrongHit()
        {
            int additionalDamage = 0;
            int coefficientIncrease = 2;

            if (RandomlyApplyAction(0, 3, 0) == true)
            {
                additionalDamage = (Attack * coefficientIncrease) - Attack;
                OutputColorText($"{Name} применил сильный удар. Дополнительный урон составил ", "", additionalDamage.ToString());
            }

            return additionalDamage;
        }

        private int CriticalHit()
        {
            int additionalDamage = 0;
            int coefficientIncrease = 10;

            if (RandomlyApplyAction(0, 100, 0) == true)
            {
                additionalDamage = (Attack * coefficientIncrease) - Attack;
                OutputColorText($"{Name} применил критический удар. Дополнительный урон составил ", "", additionalDamage.ToString());
            }

            return additionalDamage;
        }
    }

    class Magician : Fighter
    {
        private int _returnedDamage = 0;

        public Magician() : base("Маг", 100, 30, 50, 2) { }

        public override void TakeDamage(int attack)
        {
            CalculateDamageReturned(attack);
            base.TakeDamage(attack);
        }

        public override int ReturnDamage()
        {
            CurrentDamage = 0;

            for (int i = 0; i < AttackSpeed; i++)
            {
                CurrentDamage += Attack;
            }

            CurrentDamage += ReturnDamage();
            return base.ReturnDamage();
        }

        private void CalculateDamageReturned(int attack)
        {
            double returnRatio = 0.2;
            _returnedDamage = (int)(attack * returnRatio);
        }

        private int ReturnDamage()
        {
            OutputColorText($"{Name} возвращает урон равный ", "", _returnedDamage.ToString());
            return _returnedDamage;
        }
    }

    class Healer : Fighter
    {
        public Healer() : base("Знахарь", 100, 7, 40, 5) { }

        public override void TakeDamage(int attack)
        {
            SuperRegenerate(attack);
            PartialRegenerate(attack);
            base.TakeDamage(attack);
        }

        public override int ReturnDamage()
        {
            CurrentDamage = 0;

            for (int i = 0; i < AttackSpeed; i++)
            {
                CurrentDamage += Attack;
            }

            return base.ReturnDamage();
        }

        private void SuperRegenerate(int attack)
        {
            if (RandomlyApplyAction(0, 5, 0) == true)
            {
                Health += attack;
                OutputColorText($"{Name} примененил регенерацию и восстановил ", " здоровья", attack.ToString());
            }
        }

        private void PartialRegenerate(int attack)
        {
            int restoredHealth = attack / 4;
            Health += restoredHealth;
            OutputColorText($"{Name} примененил регенерацию и восстановил", " здоровья", restoredHealth.ToString());
        }
    }

    class Clever : Fighter
    {
        private int _stageStudy = 0;
        private int _limitStudy = 100;

        public Clever() : base("Ловкий", 100, 0, 0, 1) { }

        public override void TakeDamage(int attack)
        {
            DodgeBlow(attack);
            base.TakeDamage(attack);
        }

        public override int ReturnDamage()
        {
            CurrentDamage = 0;

            for (int i = 0; i < AttackSpeed; i++)
            {
                CurrentDamage += Attack;
            }

            CurrentDamage += FindWeakSpot();
            return base.ReturnDamage();
        }

        private int HittingWeakSpot()
        {
            int damage = 1000;
            OutputColorText("", "", $"Ловкий нашёл слабое место и наносит критический урон");
            return damage;
        }

        private int FindWeakSpot()
        {
            int damage = 0;
            Random Random = new Random();
            int minSpeedStudy = 0;
            int maxSpeedStudy = 20;
            int speedStudy = Random.Next(minSpeedStudy, maxSpeedStudy);
            _stageStudy += speedStudy;
            OutputColorText($"Ловкий изучил противника на ", " процентов", _stageStudy.ToString());

            if (_stageStudy >= _limitStudy)
            {
                damage = HittingWeakSpot();
            }

            return damage;
        }

        private void DodgeBlow(int attack)
        {
            if (RandomlyApplyAction(0, 5, 0) != true)
            {
                Health += attack;
                Console.WriteLine($"Ловкий уворачивается от аттаки");
            }
            else
            {
                Console.WriteLine($"Отвернуться не удалось. Урон принят.");
            }
        }
    }

    class Huskar : Fighter
    {
        private double _initialHealth;

        public Huskar() : base("Хускар", 150, 50, 0, 1)
        {
            _initialHealth = Health;
        }

        public override int ReturnDamage()
        {
            CurrentDamage = 0;

            for (int i = 0; i < AttackSpeed; i++)
            {
                CurrentDamage += Attack + IncreaseDamage();
            }

            return base.ReturnDamage();
        }

        private int IncreaseDamage()
        {
            double currentHealth = Health;
            int additionalDamage;
            double damageIncreaseFactor = (1 - (currentHealth / _initialHealth)) + 1;
            additionalDamage = (int)((Attack * damageIncreaseFactor) - Attack);
            OutputColorText($"Дополнительный урон хускара, составил ", additionalDamage.ToString());
            return additionalDamage;
        }
    }

    class Arena
    {
        private List<Fighter> _opponents = new List<Fighter>();

        public void StartGame()
        {
            bool isWork = true;

            while (isWork == true)
            {
                Console.WriteLine("1. Выбрать бойцов для боя \n2. Начать бой \n2. Выход");

                switch (Console.ReadLine())
                {
                    case "1":
                        ChooseOpponents();
                        break;

                    case "2":
                        StartFight();
                        break;

                    case "3":
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("\nНеккоректный ввод\n");
                        break;
                }
            }
        }

        private void ChooseOpponents()
        {
            _opponents.Clear();
            Console.WriteLine("\nВыберите первого бойца");
            ShowChooseMenu();
            Console.WriteLine("\nВыберите второго бойца");
            ShowChooseMenu();
            Console.WriteLine("\nГотово!\n");
        }

        private void ShowChooseMenu()
        {
            bool isWork = true;

            while (isWork == true)
            {
                Console.WriteLine("\n1. Тяжёлый \n2. Знахарь \n3. Маг \n4. Ловкий \n5. Хускар \n6. Выход\n");
                switch (Console.ReadLine())
                {
                    case "1":
                        CreateFighter(new Heavy(), ref isWork);
                        break;

                    case "2":
                        CreateFighter(new Healer(), ref isWork);
                        break;

                    case "3":
                        CreateFighter(new Magician(), ref isWork);
                        break;

                    case "4":
                        CreateFighter(new Clever(), ref isWork);
                        break;

                    case "5":
                        CreateFighter(new Huskar(), ref isWork);
                        break;

                    case "6":
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("\nНеккоректный ввод\n");
                        break;
                }
            }
        }

        private void CreateFighter(object Fighter, ref bool isWork)
        {
            _opponents.Add((Fighter)Fighter);
            isWork = false;
        }

        private void StartFight()
        {
            if (_opponents.Count == 2)
            {
                int firstFighter = 0;
                int secondFighter = 1;
                FlipСoin(ref firstFighter, ref secondFighter);
                _opponents[firstFighter].OutputColorText("", "", "", "Бой начался \n\n");

                while (_opponents[0].Health > 0 & _opponents[1].Health > 0)
                {
                    _opponents[firstFighter].ShowStats();
                    _opponents[secondFighter].ShowStats();
                    _opponents[firstFighter].TakeDamage(_opponents[secondFighter].ReturnDamage());
                    _opponents[secondFighter].TakeDamage(_opponents[firstFighter].ReturnDamage());
                    Console.WriteLine("");
                }

                _opponents[firstFighter].OutputColorText("", "", "", "\nБой закончился \n");

                foreach (var fighter in _opponents)
                {
                    if (fighter.Health > 0)
                    {
                        Console.WriteLine($"\nПобедил: {fighter.Name}\n");
                    }
                }
            }
            else
            {
                Console.WriteLine($"\nВыберите бойцов для начала боя\n");
            }
        }

        private void FlipСoin(ref int firstFighter, ref int secondFighter)
        {
            Random random = new Random();
            int minLimit = 0;
            int maxLimit = 2;
            int randomNumber = random.Next(minLimit, maxLimit);

            if (randomNumber == 0)
            {
                firstFighter = 0;
                secondFighter = 1;
            }
            else
            {
                firstFighter = 1;
                secondFighter = 0;
            }
        }
    }

}