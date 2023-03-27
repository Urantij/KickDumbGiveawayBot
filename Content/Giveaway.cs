using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Content;

public class Giveaway
{
    public readonly int dbId;

    public readonly string codeWord;

    readonly List<User> users = new();

    public Giveaway(int dbId, string codeWord)
    {
        this.dbId = dbId;
        this.codeWord = codeWord;
    }

    /// <summary>
    /// Если юзер есть, добавит очки спама и вернёт тру.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool TryUpdateUser(long id)
    {
        lock (users)
        {
            User? user = users.FirstOrDefault(u => u.id == id);

            if (user != null)
            {
                user.spamCount++;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Попытается добавить юзера. Нулл, если такой уже есть.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="isSpecial"></param>
    /// <returns></returns>
    public User? AddUser(long id, string name, bool isSpecial)
    {
        lock (users)
        {
            User? user = users.FirstOrDefault(u => u.id == id);

            if (user == null)
            {
                user = new User(id, name, isSpecial)
                {
                    spamCount = 1
                };

                users.Add(user);

                return user;
            }
            else
            {
                user.spamCount++;

                return null;
            }
        }
    }

    public int GetCount()
    {
        lock (users)
        {
            return users.Count;
        }
    }

    public User GetUser(int index)
    {
        lock (users)
        {
            return users[index];
        }
    }

    public User[] GetUsers()
    {
        lock (users)
        {
            return users.ToArray();
        }
    }
}
