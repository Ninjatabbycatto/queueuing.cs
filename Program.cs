namespace queueing;

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.Design;

public class Server
{
    public int Num { get; set; }
    public int GLength { get; set; }
    public bool IsServing { get; set; }
    public Queue<int> TaskQueue { get; set; }
    public Server(int num){
        this.Num = num;
        this.IsServing = false;
        this.GLength = 0;
        this.TaskQueue = new Queue<int>();

    }
}


public class Person {
    public int Num { get; set; }
    public int Ai { get; set; }
    public int Di { get; set; } 
    public int Si { get; set; }
    public int SrvNum { get; set; }
    public Person (int Num, int Ai, int Di, int Si, int SrvNum) {
        this.Num = Num;
        this.Ai = Ai;   //arrival time
        this.Di = Di;   //delay time
        this.Si = Si;   //service time
        this.SrvNum = SrvNum;
    }
}


public class Program{
    private static Server[] servers = Array.Empty<Server>();
    private static Person[] people = Array.Empty<Person>();


    static void Title(){
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("Queueing Sim");
    }

    static int FindSrvNum() {   //returns server with shortest queue
        int toreturn = 1;
        int shortest = 10000;
        //Console.WriteLine("----------------");
        foreach (Server server in servers) {
            if (server.TaskQueue.Count < shortest) {
                //Console.WriteLine(server.TaskQueue.Count);
                shortest = server.TaskQueue.Count;
                toreturn = server.Num;
            }
        }
        //Console.WriteLine("----------------");
        //Console.WriteLine("toReturn: " + toreturn);
        return toreturn;
    }

    static int ErlyPrson(Person person) {
        int servernum = FindSrvNum();
        person.SrvNum = servernum;
        if (servers[person.SrvNum].TaskQueue.Count > 0 ) {
            //Console.WriteLine(servers[person.SrvNum].TaskQueue.Last());
            return servers[person.SrvNum].TaskQueue.Last();

        }
        else return 0;
    }

    static int getDelayAve(Server server) {
        int total = 0;
        foreach (int person in server.TaskQueue) {
            total += people[person].Di;
        }

        int ave;
        ave = total / server.TaskQueue.Count;

        return ave;
    }
    static int getServiceTimeAve(Server server) {
        int total = 0;
        foreach (int person in server.TaskQueue) {
            total += people[person].Si;
        }

        int ave = total / server.TaskQueue.Count;
        return ave;
    }
    

    static void AddToServerQ(Person person) {
        servers[person.SrvNum].TaskQueue.Enqueue(person.Num);
    }


    static void RandomPeople(int Quant) {
        Random random = new Random();

        people = new Person[Quant];

        for (int i = 0; i < Quant; i++) { 
            int serverNum = FindSrvNum();
            if (servers[serverNum].TaskQueue.Count == 0) {     
                people[i] = new Person(i, random.Next(1, 3) , 0, random.Next(2,5), serverNum);
                AddToServerQ(people[i]);
            }

            else {
                people[i] = new Person(i, 0, 0, random.Next(2,5), serverNum);
                people[i].Ai = random.Next(1, 3) + people[ErlyPrson(people[i])].Ai;
                people[i].Di = people[ErlyPrson(people[i])].Ai + 
                            people[ErlyPrson(people[i])].Di +
                            people[ErlyPrson(people[i])].Si - people[i].Ai;
                if (people[i].Di < 0) {
                    people[i].Di = 0;
                }
                AddToServerQ(people[i]);
            }

        }
        
    }

    static void CustomPeople(int Quant) {
        people = new Person[Quant];
        //manual assignmetn of values
        people[0] = new Person(0, 15, 0, 43, 0);
        people[1] = new Person(1, 47, 0, 36, 0);
        people[2] = new Person(2, 71, 0, 34, 0);
        people[3] = new Person(3, 111, 0, 30, 0);
        people[4] = new Person(4, 123, 0, 38, 0);
        people[5] = new Person(5, 152, 0, 40, 0);
        people[6] = new Person(6, 166, 0, 31, 0);
        people[7] = new Person(7, 226, 0, 29, 0);
        people[8] = new Person(8, 310, 0, 36, 0);
        people[9] = new Person(9, 320, 0, 30, 0);

        foreach (Person person in people) {
            int serverNum = FindSrvNum();
            
            if (servers[serverNum].TaskQueue.Count == 0) {
                person.Di = 0;
                person.SrvNum = serverNum;
                AddToServerQ(person);
            }
            else {
                person.Di = people[ErlyPrson(person)].Ai + 
                people[ErlyPrson(person)].Di + 
                people[ErlyPrson(person)].Si;
                person.Di -= person.Ai;
                if (person.Di < 0) {
                    person.Di = 0;
                }
                person.SrvNum = serverNum;
                AddToServerQ(person);
            }
        }
    }

    static void printTables() {
        Title();
        foreach(Server server in servers) {
            int i = 0;
            
            Console.SetCursorPosition(0, 2 + server.Num * 7);
            Console.WriteLine("t\n\nAi\nDi\nSi");
            foreach (int waiting in server.TaskQueue) {
                Console.SetCursorPosition(5 + i * 6, 2 + server.Num * 7);
                Console.Write(people[waiting].Num);
                Console.SetCursorPosition(5 + i * 6, 3 + server.Num * 7);
                Console.Write("---------");
                Console.SetCursorPosition(5 + i * 6, 4 + server.Num * 7);
                Console.Write(people[waiting].Ai);
                Console.SetCursorPosition(5 + i * 6, 5 + server.Num * 7);
                Console.Write(people[waiting].Di);
                Console.SetCursorPosition(5 + i * 6, 6 + server.Num * 7);
                Console.Write(people[waiting].Si);
                i++;

            }
        }
    }


    static void PrintServerAve() {
        for (int i = 0; i < servers.Length; i++) {
            Console.SetCursorPosition(3, 44 + i);
            Console.Write("Server " + servers[i].Num + "    Wating ave: " 
                          + getDelayAve(servers[i])
                          + "      Service Time Ave: " + getServiceTimeAve(servers[i]));
        }
    }

    static void Main(string[] args){
        Console.Clear();
        int serverCount = 2;
        int quantity = 10;
        int interval = 1 ;


        
        servers = new Server[serverCount];
        //create servers
        for (int i = 0; i < serverCount; i++){
            servers[i] = new Server(i);
        }
        //comment out ung hindi pinili. Either randompeople or custompeople
        RandomPeople(quantity);
        //CustomPeople(10);

        foreach (var server in servers){

        }
        


        int time = 0;
        double delayAve = people.Average(person => person.Di);
        do {
            foreach (Server server in servers) {
                Console.SetCursorPosition(3, (servers.Length * 8) + server.Num);
                Console.Write(server.Num);
                int q = 0;
                foreach (int person in server.TaskQueue) {
                    printTables();



                    PrintServerAve();
                    Console.SetCursorPosition(3, (servers.Length * 8) + 5);
                    Console.Write("Time: " + time 
                                  + "      TimeToFinish: " + (people[people.Length - 1].Si + people[people.Length - 1].Di + people[people.Length - 1].Ai)
                                  + "\n---------------------------");

                    Console.SetCursorPosition(3, (servers.Length * 8) + 8 + (server.Num * 4) - 1);
                    Console.Write("Server: "  + server.Num);

                    if (time >= people[person].Ai + people[person].Di && time <= people[person].Ai + people[person].Di + people[person].Si) {
                        Console.SetCursorPosition(5,  (servers.Length * 8) + server.Num);
                        Console.Write("x");
                        
                        Console.SetCursorPosition(3, (servers.Length * 8) + 8 + (server.Num * 4));
                        Console.Write("Person " + people[person].Num + " goes to server " + server.Num 
                                      + " since queue is empty and will be there until t = " 
                                      + (people[person].Ai + people[person].Di + people[person].Si));
                        if (q > 0) q--;

                    }
                    
                    else if (time >= people[person].Ai && people[person].Ai + people[person].Di + people[person].Si > time){
                        Console.SetCursorPosition(7 + q,  (servers.Length * 8) + server.Num);
                        Console.Write("x");

                        Console.SetCursorPosition(3, (servers.Length * 8) + 9 + (server.Num * 4));
                        Console.Write("person " + people[person].Num + " will fall in line");

                        q++;
                    }
                    else if (time > people[person].Ai + people[person].Di) {

                    }
                }
            }
            
            //Thread.Sleep(speed);
            

            while(true) {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.LeftArrow) {
                    time -= interval;
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow){
                    time += interval;
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Escape){
                    Environment.Exit(0);
                }
            }

            Console.Clear();
        } while (time < 1000);
        
    }
}