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
            OutputColorText($"{Name}: ", $" HP, {Armor} ARM", "",Health.ToString());
        }

        public void OutputColorText(string textLeft, string textRight, string redText = "", string greenText = "")
        {
            Console.Write($"{textLeft}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(redText);
            Console.ForegroundColor = ConsoleColor.Green;
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
                CurrentDamage += Attack + HitHard() + HitCritical();
            }

            return base.ReturnDamage();
        }

        private int HitHard()
        {
            int additionalDamage = 0;
            int coefficientIncrease = 2;

            if (RandomlyApplyAction(30) == true)
            {
                additionalDamage = (Attack * coefficientIncrease) - Attack;
                OutputColorText($"{Name} применил сильный удар. Дополнительный урон составил ", "", additionalDamage.ToString());
            }

            return additionalDamage;
        }

        private int HitCritical()
        {
            int additionalDamage = 0;
            int coefficientIncrease = 10;

            if (RandomlyApplyAction(1) == true)
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

            CurrentDamage += RevertDamage();
            return base.ReturnDamage();
        }

        private void CalculateDamageReturned(int attack)
        {
            double returnRatio = 0.2;
            _returnedDamage = (int)(attack * returnRatio);
        }

        private int RevertDamage()
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
            if (RandomlyApplyAction(25) == true)
            {
                Health += attack;
                OutputColorText($"{Name} примененил регенерацию и восстановил ", " здоровья", attack.ToString());
            }
        }

        private void PartialRegenerate(int attack)
        {
            int recoveryFactor = 4;
            int restoredHealth = attack / recoveryFactor;
            Health += restoredHealth;
            OutputColorText($"{Name} примененил регенерацию и восстановил ", " здоровья", restoredHealth.ToString());
        }
    }

    class Clever : Fighter
    {
        private int _stageStudy = 0;
        private int _limitStudy = 100;

        public Clever() : base("Ловкий", 100, 0, 0, 1) { }

        public override void TakeDamage(int attack)
        {
            base.TakeDamage(DodgeBlow(attack));
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

        private int DodgeBlow(int attack)
        {
            int damage = 0;

            if (RandomlyApplyAction(25) != true)
            {
                Console.WriteLine($"Ловкий уворачивается от аттаки");
            }
            else
            {
                damage = attack;
                Console.WriteLine($"Отвернуться не удалось. Урон принят.");
            }

            return damage;
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
            OutputColorText($"Дополнительный урон хускара, составил ", "", additionalDamage.ToString());
            return additionalDamage;
        }
    }

    class Arena
    {
        private int _firstFighter;
        private int _secondFighter;
        private bool fightersPicked = false;

        private List<Fighter> _fighters = new List<Fighter>();

        public void StartGame()
        {
            bool isWork = true;

            while (isWork == true)
            {
                Console.WriteLine("1. Выбрать бойцов для боя \n2. Начать бой \n2. Выход");

                switch (Console.ReadLine())
                {
                    case "1":
                        ChooseFighters();
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

        private void StartFight()
        {
            if (fightersPicked == true)
            {
                _fighters[_firstFighter].OutputColorText("", "", "", "Бой начался \n\n");

                while (_fighters[_firstFighter].Health > 0 & _fighters[_secondFighter].Health > 0)
                {
                    _fighters[_firstFighter].ShowStats();
                    _fighters[_secondFighter].ShowStats();
                    _fighters[_firstFighter].TakeDamage(_fighters[_secondFighter].ReturnDamage());
                    _fighters[_secondFighter].TakeDamage(_fighters[_firstFighter].ReturnDamage());
                    Console.WriteLine("");
                }

                _fighters[_firstFighter].OutputColorText("", "", "", "\nБой закончился \n");
                AnnounceWinner(_fighters[_firstFighter].Health);
            }
            else
            {
                Console.WriteLine($"\nВыберите бойцов для начала боя\n");
            }
        }

        private void AssignFighters(ref int fighter)
        {
            bool isWork = true;

            while (isWork == true)
            {
                for (int i = 0; i < _fighters.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {_fighters[i].Name}");
                }

                bool isNumber = int.TryParse(Console.ReadLine(), out int numberFighter);

                if (isNumber == true && numberFighter <= _fighters.Count && numberFighter > 0)
                {
                    fighter = numberFighter - 1;
                    isWork = false;
                }
                else
                {
                    Console.WriteLine("\nВы ввели не число или данного бойца нет в списке\n");
                }
            }
        }

        private void ChooseFighters()
        {
            CreateFighters();
            Console.WriteLine("\nВыберите бойца слева\n");
            AssignFighters(ref _firstFighter);
            Console.WriteLine("\nВыберите бойца справа\n");
            AssignFighters(ref _secondFighter);
            _fighters[0].OutputColorText("", "", "", "\nГотово\n");
            fightersPicked = true;
        }

        private void CreateFighters()
        {
            _fighters.Clear();
            _fighters.Add(new Heavy());
            _fighters.Add(new Magician());
            _fighters.Add(new Healer());
            _fighters.Add(new Clever());
            _fighters.Add(new Huskar());
        }

        private void AnnounceWinner(int healthFirstFighter)
        {
            if (healthFirstFighter > 0)
            {
                _fighters[0].OutputColorText("\nПобедил ","", "", _fighters[_firstFighter].Name);
            }
            else
            {
                _fighters[0].OutputColorText("\nПобедил ", "", "", _fighters[_secondFighter].Name);
            }
        }
    }
}