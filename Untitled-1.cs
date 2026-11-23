/*

1.1

1) Чтобы потом можно было добавить другие типы объектов, которым можно наносить урон (они будут реализовывать интерфейс)
2) Нет, контракт не предполагает изменение конкретных переменных или возвращение True
3) Зелье нельзя было бы передать в список т.к. оно не реализует IDamageable (а ещё это было бы странно с точки зрения игровой логики)
4) Придется изменить все классы реализующие IDamageable (Trap и Player), иначе будет ошибка что контракт интерфейса не реализован

1.2

1) +- абстрактыный класс может задавать полям только значения по умолчанию, а интерфес - не может никакие
2) +- абстрактный класс вообще может иметь полноценные функции (не только абстрактные). А экземпляр интерфейса создать нельзя, поэтому и полноценный конструктор прописать тоже.
3) -+ нет, нельзя наследоваться от нескольких классов, но можно реализовывть несколько интерфейсов
4) +- абстрактный класс вообще может иметь полноценные функции (не только абстрактные). В интерфейсе тело функции прописать нельзя.
5) ++ И там и там можно пользоваться автосвойствами, например
6) -+ Только обычный класс можно использовать как тип. Абстрактный - нет
7) +- наследовать классы может только... класс, как ни странно
8) ++ технически интерфейс может их содержать, но в этом нет особенного смысла
9*) Player : IDamageable, IDialougueInteractable, IHealable
    NPC : IDamageable, IDialougueInteractable
    Enemy : IDamageable

2.1

using System;

public interface ICombatant
{
    string Name { get; }
    int HP { get; set; }
    bool IsDead { get; }
    int Damage { get; }

    void Attack(ICombatant target);
    void TakeDamage(int amount);
}

public class Knight : ICombatant
{
    public string Name { get; }
    public int HP { get; set; } = 150;
    public int Damage { get; } = 25;

    public bool IsDead => HP <= 0;

    public Knight(string name)
    {
        Name = name;
    }

    public void Attack(ICombatant target)
    {
        if (IsDead)
        {
            Console.WriteLine($"Рыцарь {Name} не может атаковать, так как мёртв.");
            return;
        }

        if (target.IsDead)
        {
            Console.WriteLine($"Рыцарь {Name} не может атаковать мёртвую цель.");
            return;
        }

        Console.WriteLine($"Рыцарь {Name} атакует!");
        target.TakeDamage(Damage);
    }

    public void TakeDamage(int amount)
    {
        HP = Math.Max(0, HP - amount);

        Console.WriteLine(
            $"Рыцарь {Name} получил {amount} урона. Осталось HP: {HP}"
        );
    }
}

public class Mage : ICombatant
{
    public string Name { get; }
    public int HP { get; set; } = 80;
    public int Damage { get; } = 40;

    public bool IsDead => HP <= 0;

    public Mage(string name)
    {
        Name = name;
    }

    public void Attack(ICombatant target)
    {
        if (IsDead)
        {
            Console.WriteLine($"Маг {Name} не может атаковать, так как мёртв.");
            return;
        }

        if (target.IsDead)
        {
            Console.WriteLine($"Маг {Name} не может атаковать мёртвую цель");
            return;
        }

        Console.WriteLine($"Маг {Name} использует двойное заклинание!");

        target.TakeDamage(Damage);

        if (!target.IsDead)
        {
            target.TakeDamage(Damage);
        }
    }

    public void TakeDamage(int amount)
    {
        HP = Math.Max(0, HP - amount);

        Console.WriteLine(
            $"Маг {Name} получил {amount} урона. Осталось HP: {HP}"
        );
    }
}

public class Program
{
    public static void Main()
    {
        Knight knight = new Knight("Артур");
        Mage mage = new Mage("Мерлин");

        while (!knight.IsDead && !mage.IsDead)
        {
            knight.Attack(mage);

            if (mage.IsDead)
                break;

            mage.Attack(knight);
        }

        if (knight.IsDead)
        {
            Console.WriteLine($"Победил Маг {mage.Name}!");
        }
        else
        {
            Console.WriteLine($"Победил Рыцарь {knight.Name}!");
        }
    }
}

2.2

using System;
using System.Collections.Generic;

public enum Rarity
{
    Common,
    Rare,
    Legendary
}

public interface IUsableItem
{
    string Name { get; }
    string Description { get; }
    Rarity Rarity { get; }
    void Use(Player player);
}

public class Player
{
    public string Name { get; }
    public int HP { get; private set; }
    public int Damage { get; private set; }

    public Player(string name, int hp, int damage)
    {
        Name = name;
        HP = hp;
        Damage = damage;
    }

    public void Heal(int amount)
    {
        HP += amount;
    }
    public void IncreaseDamage(int amount)
    {
        Damage += amount;
    }
    public void TakeDamage(int amount)
    {
        HP = Math.Max(0, HP - amount);
    }
}

public class HealthPotion : IUsableItem
{
    public string Name { get; } = "Зелье здоровья";
    public string Description { get; } = "Восстанавливает 50 HP";
    public Rarity Rarity { get; } = Rarity.Common;

    public void Use(Player player)
    {
        player.Heal(50);
        Console.WriteLine("Вы выпили зелье и восстановили 50 HP!");
    }
}

public class Sword : IUsableItem
{
    public string Name { get; } = "Меч";
    public string Description { get; } = "Увеличивает урон на 10";
    public Rarity Rarity { get; } = Rarity.Rare;

    public void Use(Player player)
    {
        player.IncreaseDamage(10);
        Console.WriteLine("Вы взяли меч и увеличили свой урон на 10!");
    }
}

public class AncientScroll : IUsableItem
{
    public string Name { get; } = "Древний свиток";
    public string Description { get; } = "Наносит вам 20 урона, но увеличивает урон на 30";
    public Rarity Rarity { get; } = Rarity.Legendary;

    public void Use(Player player)
    {
        player.TakeDamage(20);
        player.IncreaseDamage(30);

        Console.WriteLine(
            "Вы использовали древний (проклятый) свиток! Получено 20 урона, но ваш урон увеличился на 30!"
        );
    }
}

public class Program
{
    public static void Main()
    {
        Player player = new Player("Артур II", 100, 10);

        List<IUsableItem> items = new List<IUsableItem>
        {
            new HealthPotion(),
            new Sword(),
            new AncientScroll()
        };

        foreach (IUsableItem item in items)
        {
            item.Use(player);
        }

        Console.WriteLine($"Имя: {player.Name}");
        Console.WriteLine($"HP: {player.HP}");
        Console.WriteLine($"Урон: {player.Damage}");

        if (player.HP > 0)
        {
            Console.WriteLine("Игрок жив");
        }
        else
        {
            Console.WriteLine("Игрок мёртв");
        }
    }
}

2.3
*/
using System;

public interface IDamageCalculator
{
    int CalculateDamage(int baseDamage, int targetArmor);
}

public class PhysicalDamageCalculator : IDamageCalculator
{
    public int CalculateDamage(int baseDamage, int targetArmor)
    {
        return Math.Max(1, baseDamage - targetArmor / 2);
    }
}

public class MagicDamageCalculator : IDamageCalculator
{
    public int CalculateDamage(int baseDamage, int targetArmor)
    {
        int damage = Math.Max(1, baseDamage - targetArmor / 4);
        if (targetArmor == 0)
        {
            damage *= 2;
        }
        return damage;
    }
}

public class FireDamageCalculator : IDamageCalculator
{
    public int CalculateDamage(int baseDamage, int targetArmor)
    {
        int damage = Math.Max(1, baseDamage - targetArmor / 10);
        if (baseDamage > 50)
        {
            damage += 10;
        }
        return damage;
    }
}

public class Unit
{
    public string Name { get; }
    public int HP { get; set; }
    public int Armor { get; }

    public Unit(string name, int hp, int armor)
    {
        Name = name;
        HP = hp;
        Armor = armor;
    }
}

public class DamageSystem
{
    private IDamageCalculator calculator;

    public DamageSystem(IDamageCalculator calculator)
    {
        this.calculator = calculator;
    }

    public void ApplyDamage(Unit target, int baseDamage)
    {
        int damage = calculator.CalculateDamage(
            baseDamage,
            target.Armor
        );

        target.HP = Math.Max(0, target.HP - damage);

        Console.WriteLine(
            $"Юнит {target.Name} получил {damage} урона. Осталось HP: {target.HP}"
        );
    }
}


public class Program
{
    public static void Info(Unit unit)
    {
        Console.WriteLine($"Юнит: {unit.Name}");
        Console.WriteLine($"HP: {unit.HP}");
        Console.WriteLine($"Броня: {unit.Armor}");
    }   
    public static void Main()
    {
        Unit unit = new Unit("Артур III", 300, 20);

        int baseDamage = 60;

        Info(unit);

        Console.WriteLine("Физический урон");
        DamageSystem physicalSystem = new DamageSystem(new PhysicalDamageCalculator());

        physicalSystem.ApplyDamage(unit, baseDamage);
        Info(unit);

        Console.WriteLine("Магический урон");
        DamageSystem magicSystem = new DamageSystem(new MagicDamageCalculator());

        magicSystem.ApplyDamage(unit, baseDamage);
        Info(unit);

        Console.WriteLine("Огненный урон");
        DamageSystem fireSystem = new DamageSystem(new FireDamageCalculator());

        fireSystem.ApplyDamage(unit, baseDamage);
        Info(unit);

        Console.WriteLine("Результат");
        Info(unit);

        if (unit.HP > 0)
            Console.WriteLine("Челик жив");
        else
            Console.WriteLine("Челик мёртв");
    }
}