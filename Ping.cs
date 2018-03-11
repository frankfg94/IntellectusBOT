﻿using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Linq;
using Discord.WebSocket;

namespace BT
{
   
    public class Ping : ModuleBase<SocketCommandContext>
    {

        async Task<int> LoadDataAsync()
        {
            await Task.Delay(2000);
            return 42;
        }
        private readonly DiscordSocketClient _client;
        public Ping(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("a")]
        public async Task ObtainTheFileAsync()
        {
            await ReplyAsync("Je vous ai ouvert une fenêtre");
            var progressForm = new Form()
            {
                Width = 300,
                Height = 200,
                Text = "Charger un fichier... "
            };
            TextBox txtBox = new TextBox();
            txtBox.Location = new Point(10, 50);
            txtBox.Visible = true;
            progressForm.Controls.Add(txtBox);
            var progressFormTask = progressForm.ShowDialog();
            var data = await LoadDataAsync();
            progressForm.Close();
            MessageBox.Show(data.ToString());
        }

        [Command("userList")]
        public async Task DisplayUsers()
        {

            var users = Context.Guild.Users;
            foreach (var u in users)
                if (!u.IsBot && u.Id != 353243323592605699) 
                {
                    await u.ModifyAsync(x => x.Nickname = "[Test]" + u.Username);
                    await ReplyAsync(u.Username);
                }

            if (users.Count == 4)
            {
                await ReplyAsync("\nBien il semblerait que nous sommes tous présents, car nous sommes 4!");
            }
        }

        private int result;
        private Random n = new Random(Guid.NewGuid().GetHashCode());
        static bool responded = false;


        [Command("dice")]
        public async Task Dice([Remainder] int max = 6)
        {
            var EmbedBuilder = new EmbedBuilder();
            EmbedBuilder.WithColor(Discord.Color.Blue);
            EmbedBuilder.WithTitle("Vous avez lancé un dé");
            Random r = new Random();
            EmbedBuilder.AddField("Résultat : ", r.Next(0, max));
            await Context.Channel.SendMessageAsync("", false, EmbedBuilder);
        }

        [Command("qcm2  ")]
        public async Task qcm2()
        {
            var eb = new EmbedBuilder();
            var eb2 = new EmbedBuilder();
            var eb3 = new EmbedBuilder();
            var eb4 = new EmbedBuilder();
            Random n = new Random();
            responded = true;
            eb.WithTitle("QCM visuel");
            eb.WithDescription("Selectionner la réponse appropriée");
            eb.AddField("a) ", "1").WithImageUrl("http://www.clker.com/cliparts/c/2/4/3/1194986855125869974rubik_s_cube_random_petr_01.svg.med.png");
            eb2.AddField("b) ", "2").WithImageUrl("http://image.jeuxvideo.com/medias-md/145528/1455277964-3848-card.jpg");
            eb3.AddField("c) ", "3").WithImageUrl("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTEOleL54qD1hv-Y4KwzHLnWo9BGyUE3g8vYn4qWD8Lfk2i4-DJug");
            eb4.AddField("d) ", "4").WithImageUrl("http://popupcity.net/wp-content/uploads/2012/04/CityEngine.jpg");
            var m1 = await Context.Channel.SendMessageAsync("", false, eb);
            var m2 = await Context.Channel.SendMessageAsync("", false, eb2);
            var m3 = await Context.Channel.SendMessageAsync("", false, eb3);
            var m4 = await Context.Channel.SendMessageAsync("", false, eb4);
            await AddQcmReactions(m1);
            await AddQcmReactions(m2);
            await AddQcmReactions(m3);
            await AddQcmReactions(m4);            
        }

        public async Task AddQcmReactions(IUserMessage msg)
        {
            await msg.AddReactionAsync(new Emoji("🇦"));
            await msg.AddReactionAsync(new Emoji("🇧"));
            await msg.AddReactionAsync(new Emoji("🇨"));
            await msg.AddReactionAsync(new Emoji("🇩"));
        }

        public async Task GetAllParameters(object b)
        {
            var properties = b.GetType().GetProperties();
            foreach (var p in properties)
            {
                string name = p.Name;
                object value = p.GetValue(b);
                await ReplyAsync(name + " = " + value);
            }
        }

        [Command("info", RunMode = RunMode.Async)]
        public async Task Info([Remainder]ulong id)
        {
            var user = await Context.Channel.GetUserAsync(id);
            await GetAllParameters(user);
        }

        //[Command("IA")]
        //public async Task IA()
        //{

        //}

        [Command("rep", RunMode = RunMode.Async)]
        public async Task Answer([Remainder] string letter)
        {
            await ReplyAsync("Vous avez choisi la réponse : " + letter + ")");
            if (responded)
            {
                if (letter == "a")
                {
                    await ReplyAsync("  Bonne réponse!!");
                    responded = true;
                }
                else if (letter == "b" || letter == "c" || letter == "d")
                {
                    await ReplyAsync("  Mauvaise réponse");
                }
                else
                {
                    await ReplyAsync("Merci d'entrer a, b, c ou d");
                }
            }
            else
            {
                await ReplyAsync("Merci de d'abord lancer un QCM");
            }
        }


        [Command("clean", RunMode = RunMode.Async)]
        public async Task Clean([Remainder] uint amount)
        {
            var messages = await Context.Channel.GetMessagesAsync((int)amount + 1).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
            await ReplyAsync("Suppression de " + amount + " messages");
        }

        [Command("kill", RunMode = RunMode.Async)]
        public async Task Kill([Remainder] string name)
        {
            var users = Context.Guild.Users;
            foreach(var u in users)
            {
                if (u.Username.ToUpper() == name.ToUpper() || u.Nickname.ToUpper() == name.ToUpper())
                {
                    await u.ModifyAsync(x => x.Nickname = "[MORT]" + u.Username);
                    await ReplyAsync(":scream: " + u.Username + " vient de mourir!");
                }
            }

        }

        [Command("killID", RunMode = RunMode.Async)]
        public async Task KillID( ulong id)
        {
            SocketGuildUser user = await Context.Channel.GetUserAsync(id) as SocketGuildUser;
            await user.ModifyAsync(x => x.Nickname = "[MORT]" + user.Username);
            await ReplyAsync(":scream: " + user + " vient de mourir!");
        }

        public async Task KillUser(SocketGuildUser user)
        {
               Console.WriteLine(user.Username);
               await user.ModifyAsync(x => x.Nickname = "[MORT]" + user.Username);
               await ReplyAsync(":scream: " + user.Username + " vient de mourir!");
        }

        [Command("killAll", RunMode = RunMode.Async)]
        public async Task KillAll()
        {
            var users = Context.Guild.Users;
            string userString = null;
            foreach(var u in users)
            {
                Console.WriteLine(u);
                if (!u.IsBot && u.Id != 353243323592605699)
                {
                    await u.ModifyAsync(x => x.Nickname = "[!]" + u.Username); 
                }
            }
            await ReplyAsync(userString);
            await ReplyAsync("Bonjour, j'ai changé vos pseudos pour voir :eyeglasses: ");
        }

        [Command("resetN", RunMode = RunMode.Async)]
        public async Task ResetNames()
        {
            var users = Context.Guild.Users;
            foreach (var u in users)
            {
               if( u.Id != 353243323592605699)
                    await u.ModifyAsync(x => x.Nickname = u.Username);
            }
            await ReplyAsync("Vos pseudos sont revenus à la normale ! :eyeglasses: ");
        }

        [Command("help", RunMode = RunMode.Async)]
        public async Task ShowHelp()
        {
            var eb = new EmbedBuilder();
            var eb2 = new EmbedBuilder();
            string desc = ">> Il faut mettre un point d'exclamation en préfixe pour exécuter une commande\n:diamond_shape_with_a_dot_inside: ping: Le bot affiche 'Hello World'(edited)\n:diamond_shape_with_a_dot_inside: qcm: le bot lance un qcm très basique de mathématiques\n: diamond_shape_with_a_dot_inside: qcm2: le bot lance un qcm visuel(edited)\n:diamond_shape_with_a_dot_inside: a: (si client discord.net téléchargé uniquement) ouvre une fenêtre windows\n:diamond_shape_with_a_dot_inside: clean[nb de messages] : permet de supprimer jusqu'à 100 messages dans la conversation actuelle\n:diamond_shape_with_a_dot_inside: dice[n] : Lance un dé possédant n faces(edited)\n:diamond_shape_with_a_dot_inside: rep[a | b | c | d] : Répond à la commande !qcm en sélectionner une des lettres a, b, c ou d\n:diamond_shape_with_a_dot_inside: eb: Envoie un message test d'Embed Builder(edited)\n:diamond_shape_with_a_dot_inside: note[string] : Attribue une note sur 10 à un objet, personne entré en tant que string\n:diamond_shape_with_a_dot_inside: clip1: lance une musique via youtube directement sur le bot\n: diamond_shape_with_a_dot_inside: clip2: lance une musique via un chemin de l'ordinateur directement sur le bot(edited)\n:diamond_shape_with_a_dot_inside: info[id] : Lance le maximum d'informations sur un utilisateur dont l'ID est indiqué en paramètre" +
                "\n:diamond_shape_with_a_dot_inside: !killAll : Tue tous les utilisateurs du groupe (sauf le MJ)" +
                "\n:diamond_shape_with_a_dot_inside: !resetN : Remet les pseudos à leur état d'origine" +
                "\n:diamond_shape_with_a_dot_inside: !bigQcm : crée un qcm appelé 'Premier qcm'" +
                "\n:diamond_shape_with_a_dot_inside: !preview[nom du qcm] : affiche les questions d'un qcm ";
            string desc2 = "\n:diamond_shape_with_a_dot_inside: !start[nom du qcm] : débute un Qcm " +
            "\n:diamond_shape_with_a_dot_inside: !add[nom du qcm] : ajoute une question au Qcm en fin de liste " +
             "\n:diamond_shape_with_a_dot_inside: !addMany[nom du qcm] : ajoute 5 questions au Qcm en fin de liste " +
            "\n:diamond_shape_with_a_dot_inside:  !del[nom du qcm] : supprime la dernière question du Qcm indiqué" +
            "\n:diamond_shape_with_a_dot_inside:  !deleteQCM[nom du qcm] : c'est définitif" +
            "\n:diamond_shape_with_a_dot_inside:  !mix[nom du qcm] : mélange les questions d'un qcm ";

            eb.WithDescription(desc);
            eb2.WithDescription(desc2);
            await Context.Channel.SendMessageAsync("",false,eb);
            await Context.Channel.SendMessageAsync("", false, eb2); 
        }


        public static List<Qcm> qcmList = new List<Qcm>();

        [Command("mix", RunMode = RunMode.Async)]
        public async Task MixQCM([Remainder] string QCMname)
        {
            bool QCMfound = false;
            foreach (var qcm in qcmList)
            {
                if (qcm.name.ToUpper() == QCMname.ToUpper().Trim())
                {
                    QCMfound = true;
                    qcm.MixQuestions();
                    qcm.MixQuestions();
                    qcm.MixQuestions();
                    qcm.MixQuestions();

                    await ReplyAsync("Le QCM : " + qcm.name + " a été mélangé avec succès");
                }
            }
            if (!QCMfound)
            {
                if (qcmList.Count > 0)
                {
                    await DisplayList();
                }
                else
                {
                    await ReplyAsync(" ! Aucun QCM n'a pour le moment été crée");
                }

            }
        }

        [Command("deleteQCM", RunMode = RunMode.Async)]
        public async Task DeleteQCM([Remainder] string QCMname)
        {
            bool QCMfound = false;
            foreach(var qcm in qcmList)
            {
                if (qcm.name.ToUpper() == QCMname.ToUpper().Trim())
                {
                    QCMfound = true;
                    qcm.Delete();
                    await ReplyAsync("QCM supprimé!");
                }
            }
            if (!QCMfound)
            {
                if (qcmList.Count > 0)
                {
                    await DisplayList();
                }
                else
                {
                    await ReplyAsync(" ! Aucun QCM n'a pour le moment été crée");
                }

            }
        }

        [Command("CreateAndStart", RunMode = RunMode.Async)]
        public async Task CreateAndStart([Remainder] string name)
        {
            await BigQcm(name);
            await StartQCM(name);
        }

        [Command("BigQcm", RunMode = RunMode.Async)]
        public async Task BigQcm(string name = "a")
        {
            ISocketMessageChannel channel = Context.Channel;
            await ReplyAsync("Lancement du méga QCM! :fire: ");
            Qcm bigQcm = new Qcm();
            qcmList.Add(bigQcm);
            bigQcm.name = name;
            bigQcm.AddQuestion(QType.text);
            bigQcm.AddQuestion(QType.image); // Image multiple
            bigQcm.AddQuestion(QType.image, true, - 1); // Image simple
            bigQcm.AddQuestion(QType.image, true, -1); // Image simple
            bigQcm.AddQuestion(QType.image, true, -1); // Image simple
            bigQcm.AddQuestion(QType.image); // Image multiple
            await ReplyAsync("Questions ajoutées avec succès");
            //await Context.Guild.CreateTextChannelAsync(bigQcm.name);
            //await bigQcm.Preview(channel);
        }

        private async Task DisplayList()
        {
            string s = string.Empty;
            int i = 0;
            if (qcmList.Count() == 0)
            {
                s = "Aucun QCM n'existe pour le moment";
            }
            else
            {
                await ReplyAsync("Aucune correspondance n'a été trouvée\n Voici la liste des noms repertoriés");
                foreach (var qcm in qcmList)
                {
                    s = s + "\n" + i + ". " + qcm.name;
                    i++;
                }
            }

            await ReplyAsync(s);
        }


        [Command("preview", RunMode = RunMode.Async)]
        public async Task Preview([Remainder] string qcmName)
        {
            bool QCMfound = false;
            foreach (var qcm in qcmList)
            {
                if (qcm.name.ToUpper() == qcmName.ToUpper().Trim())
                {
                    await ReplyAsync("QCM sélectionné, prévisualisation...");
                    QCMfound = true;
                    await qcm.Preview(Context.Channel);
                }
            }
            if (!QCMfound)
            {
                if (qcmList.Count > 0)
                {
                    await DisplayList();
                }
                else
                {
                    await ReplyAsync(" ! Aucun QCM n'a pour le moment été crée");
                }
            }
        }

        [Command("add", RunMode = RunMode.Async)]
        public async Task AddQuestion([Remainder]string qcmName)
        {
            bool QCMfound = false;
            int type = QType.text;
            foreach (var qcm in qcmList)
            {
                if (qcm.name == qcmName.Trim())
                {
                    QCMfound = true;
                    qcm.AddQuestion(type);
                    string typeString = string.Empty;
                    if (type == QType.text) typeString = "Texte";
                    else if (type == QType.image) typeString = "Image";
                    else if (type == QType.audio) typeString = "Audio";
                    else  typeString = "Inconnu";
                    await ReplyAsync("Nouvelle question créee ! \n Elle de type :" + typeString);
                }
            }
            if (!QCMfound)
            {
                if (qcmList.Count > 0)
                {
                    await DisplayList();
                }
                else
                {
                    await ReplyAsync(" ! Aucun QCM n'a pour le moment été crée");
                }

            }
        }

        public async Task ReactionParse(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel msg2, SocketReaction socketReaction)
        {
            ///*await*/ ReplyAsync("Pourquoi ce message ne veut-il pas s'envoyer????");
            await Console.Out.WriteLineAsync("\n----------------------------------------------------------------------\nRéaction détectée!! " + msg.Id);
            if (qcmList != null)
            {
                foreach(var qcm in qcmList)
                {
                    if (qcm.HasStarted)
                    {
                        if (qcm.questionsID.Contains(socketReaction.MessageId) && !socketReaction.User.Value.IsBot)
                        {   
                            await Console.Out.WriteLineAsync("\nMessage cliqué appartient à liste des questions -->" );
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("True\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            await Console.Out.WriteLineAsync("On passe à Q"+ i);
                            //await ReplyAsync("Reaction détectée");
                            //await DisplayCitation();
                            await StartQCM(qcm.name);
                            i++;
                        }       
                    }
                    
                }



                // si oui, on examine si la reaction est sur le bon embed builder
                

                // si elle n'est pas la bonne réponse, on la supprime

                // si le smiley n'est pas le bon, message spécials

            }
        }

        public async Task<Qcm> GetQcm(string name)
        {
            foreach (Qcm qcm in qcmList)
            {
                if (qcm.name.ToUpper() == name.ToUpper().Trim())
                {
                    Console.WriteLine("QCM détecté :" + qcm.name);
                    return qcm;
                }
            }
            await DisplayList();
            return null;
        }

        public async Task CheckAnswer(IUserMessage msg, Qcm.Question q)
        {
            await Task.Delay(5000);
            if(msg.Reactions.Count() > 4)
            {
                var reaction = msg.Reactions.Last();
                await ReplyAsync(reaction.Value.ToString());
               
            }
        }


        [Command("citation",RunMode = RunMode.Async)]
        public async Task DisplayCitation()
        {
            var lines = File.ReadAllLines("citations.txt");
            Random r = new Random();
            var randomLineNumber = r.Next(0,lines.Length -1);
            EmbedBuilder e = new EmbedBuilder();
            e.WithTitle("Citation n°"  + randomLineNumber);
            e.WithDescription(lines[randomLineNumber]);
            await Context.Channel.SendMessageAsync("",false, e);
        }


        static int i = 1; 
        [Command("start", RunMode = RunMode.Async)]
        public async Task StartQCM([Remainder] string qcmName)
        {
            Qcm qcm = await GetQcm(qcmName);
            IMessage msg;
            if (!qcm.HasStarted)
            {
                Console.WriteLine("Affichage Q1" );
                msg = await qcm.DisplayInDiscord(_client.GetChannel(414746672284041222) as ISocketMessageChannel, qcm.questions[0]);
                qcm.questionsID.Add(msg.Id);
                qcm.HasStarted = true;
            }
            else //Problème réussir à bloquer l'affichage Q2 cad ignorer le dernier smiley
            { 
               ISocketMessageChannel channel = _client.GetChannel(414746672284041222) as ISocketMessageChannel;
                if (i < qcm.questions.Count)
                {
                    Console.WriteLine("On arrive à une question de type :" + qcm.questions[i].type);
                    msg = await qcm.DisplayInDiscord(_client.GetChannel(414746672284041222) as ISocketMessageChannel, qcm.questions[i]);
                    qcm.questionsID.Add(msg.Id);
                }
                else await channel.SendMessageAsync("Vous êtes arrivé au bout de ce QCM");
            }
        }

        [Command("addMany", RunMode = RunMode.Async)]
        public async Task AddQuestions([Remainder]string qcmName)
        {
            // problème étrange
            bool QCMfound = false;
            foreach (var qcm in qcmList)
            {
                if (qcm.name.ToUpper() == qcmName.ToUpper().Trim())
                {
                    int type = QType.text;
                    QCMfound = true;
                    string typeString = string.Empty;
                    if (type == QType.text) typeString = "Texte";
                    if (type == QType.image) typeString = "Image";
                    if (type == QType.audio) typeString = "Audio";
                    else typeString = "Inconnu";
                    qcm.AddQuestions();
                    await ReplyAsync("Questions rajoutées ! \n Type :" + typeString);
                }
            }
            if (!QCMfound)
            {
                if (qcmList.Count > 0)
                {
                    await DisplayList();
                }
                else
                {
                    await ReplyAsync(" :exclamation:  Aucun QCM n'a pour le moment été crée");
                }
            }
        }


        [Command("del", RunMode = RunMode.Async)]
        public async Task RemoveLastQuestion([Remainder] string qcmName)
        {
            bool QCMfound = false;
            foreach(var qcm in qcmList)
            {
                if(qcm.name == qcmName.Trim())
                {
                    await ReplyAsync("QCM sélectionné");
                    QCMfound = true;
                    qcm.DeleteQuestion(qcm.questions.Count-1);
                    await ReplyAsync("La question nommée : " + qcm.questions.Last().name+ " est désormais supprimée");
                }
            }
            if(!QCMfound)
            {
                if(qcmList.Count > 0)
                {
                    await DisplayList();
                }
                else
                {
                    await ReplyAsync(" ! Aucun QCM n'a pour le moment été crée");
                }

            }
        }

        [Command("ping")]
        public async Task ping()    
        {
            var msg = await ReplyAsync("Hello World");
            await msg.AddReactionAsync(new Emoji("😨"));
            await Context.Client.SetGameAsync("House Party");   
        }   

        [Command("eb")]
        public async Task beauMsg()
        {
            var eb = new EmbedBuilder();
            eb.WithDescription("Ceci est un test");
            eb.WithColor(Discord.Color.DarkBlue);
            eb.WithTitle("Message amélioré");
            await Context.Channel.SendMessageAsync("", false, eb);
        }

        [Command("note")]
        public async Task note(string s)
        {
            Random random = new Random();
            await Context.Channel.SendMessageAsync("Note : " + random.Next(0,11)+" / 10");
        }

    }

}
