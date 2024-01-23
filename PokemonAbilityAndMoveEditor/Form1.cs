using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonAbilityAndMoveEditor
{
    public partial class Form1 : Form
    {
        public string erpath;
        public List<String> pknames;
        public List<String> pknamesm;
        public Dictionary<String, int> pklines;
        public Dictionary<String, String[]> abilities;
        public Dictionary<String, String[]> innates;
        public bool debug;
        public List<String> abilitynames;
        public List<String> abilityenums;
        public List<String> movenames;
        public List<String> moveenums;
        public Dictionary<String, List<String>> levelupmoves;
        public Dictionary<String, List<String>> levelupchanges;
        bool levelup;
        public Dictionary<String, String[]> stats;

        public Form1()
        {
            
            debug = true;
            levelup = false;
            InitializeComponent();
            label2.Text = "Welcome";
            HPBAR.Maximum = 255;
            ATKBAR.Maximum = 255;
            DEFBAR.Maximum = 255;
            SPEBAR.Maximum = 255;
            SPABAR.Maximum = 255;
            SPDBAR.Maximum = 255;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (debug)
            {
                erpath = "C:\\Users\\Jadiel\\Desktop\\decomps\\eliteredux";
                initData();
                return;
            }
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Please select your folder with elite redux";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                erpath = fbd.SelectedPath;
                if (erpath != null)
                {
                    initData();
                }
            }

        }

        public void initData()
        {
            string bspath = erpath + "\\src\\data\\pokemon\\base_stats.h";
            List<String> basestatlines = new List<String>();
            try
            {
                StreamReader sr = new StreamReader(bspath);
                string line = sr.ReadLine();
                while(line != null)
                {
                    basestatlines.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Cannot read base stats file");
            }
            int i = 0;
            pknames = new List<String>();
            pklines = new Dictionary<String, int>();
            abilities = new Dictionary<String, String[]>();
            innates = new Dictionary<String, String[]>();
            stats = new Dictionary<String, String[]>();
            while (i < basestatlines.Count)
            {
                if (basestatlines[i].Contains("SPECIES_") && !basestatlines[i].Contains("SPECIES_NONE"))
                {
                    string l = basestatlines[i];
                    if(basestatlines[i + 5].Contains("= 0,")) //unfinished mon
                    {
                        i++;
                        continue;
                    }
                    String[] tstats = new string[6];
                    for(int k = 0; k < 6; k++)
                    {
                        string temp = basestatlines[k + i + 2];
                        int ik = temp.IndexOf('=');
                        int jk = temp.IndexOf(',');
                        tstats[k] = temp.Substring(ik + 1, jk - ik - 1);
                    }
                    string name = l.Replace("[SPECIES_", "").Replace("]", "").Replace("=","").Trim().ToLower();
                    name = char.ToUpper(name[0]) + name.Substring(1);
                    Console.WriteLine("Processing: " + name);
                    stats[name] = tstats;
                    pknames.Add(name);
                    pklines[name] = i;
                    int j = i;
                    while (!basestatlines[j].Contains(".abilities"))
                    {
                        j++;
                    }
                    string abs = basestatlines[j].Replace(',', ' ').Replace('{', ' ').Replace('}', ' ');
                    string inn = basestatlines[j+1].Replace(',', ' ').Replace('{', ' ').Replace('}', ' ');
                    j = 0;
                    //string[] sr = s.Split(' ');
                    string[] abss = abs.Split(' ');
                    string[] inns = inn.Split(' ');
                    string[] abies = new string[3];
                    string[] innies = new string[3];
                    foreach(string sk in abss)
                    {
                        if (sk.Contains("ABILITY"))
                        {
                            abies[j] = sk;
                            j++;
                        }
                        if(j == 3)
                        {
                            break;
                        }
                    }
                    j = 0;
                    foreach (string sk in inns)
                    {
                        if (sk.Contains("ABILITY"))
                        {
                            innies[j] = sk;
                            j++;
                        }
                        if (j == 3)
                        {
                            break;
                        }
                    }
                    abilities[name] = abies;
                    innates[name] = innies;
                }
                i++;
            }
            comboBox1.DataSource = pknames;
            initAbilities();
            initMoves();
        }

        //combobox 2,3,4 are abilities
        //combobox 5,6,7 are innates
        private void button2_Click(object sender, EventArgs e)
        {


            if(erpath == null || (comboBox1.SelectedIndex == -1))
            {
                return;
            }
            label2.Text = stripname((string)comboBox1.SelectedItem);
            loadOriginalAbilities();
            if (!label2.Text.ToLower().Contains("mega") && !label2.Text.ToLower().Contains("_blade"))
            {
                refreshLevelup();
            }
            refreshStats();
        }
        public void refreshStats()
        {
            string currentPokemon = (string)comboBox1.SelectedItem;
            String[] st = stats[currentPokemon];
            hpbox.Text = st[0];
            atkbox.Text = st[1];
            defbox.Text = st[2];
            spebox.Text = st[3];
            spabox.Text = st[4];
            spdbox.Text = st[5];
            HPBAR.Value = int.Parse(st[0].Trim());
            ATKBAR.Value = int.Parse(st[1].Trim());
            DEFBAR.Value = int.Parse(st[2].Trim());
            SPEBAR.Value = int.Parse(st[3].Trim());
            SPABAR.Value = int.Parse(st[4].Trim());
            SPDBAR.Value = int.Parse(st[5].Trim());
            updateBST();

        }
        public void loadOriginalAbilities()
        {
            string currentPokemon = (string) comboBox1.SelectedItem;
            string[] abs = abilities[currentPokemon];
            string[] inn = innates[currentPokemon];

            comboBox2.SelectedIndex = abilityenums.IndexOf(abs[0]);
            comboBox3.SelectedIndex = abilityenums.IndexOf(abs[1]);
            comboBox4.SelectedIndex = abilityenums.IndexOf(abs[2]);
            comboBox5.SelectedIndex = abilityenums.IndexOf(inn[0]);
            comboBox6.SelectedIndex = abilityenums.IndexOf(inn[1]);
            comboBox7.SelectedIndex = abilityenums.IndexOf(inn[2]);
        }
        public void initAbilities()
        {
            string abilitypath = erpath + "\\src\\data\\text\\abilities.h";
            List<String> ablines = new List<String>();
            abilityenums = new List<String>();
            abilitynames = new List<String>();
            bool f = false;
            try
            {
                StreamReader sr = new StreamReader(abilitypath);
                string line = sr.ReadLine();
                
                while (line != null)
                {
                    if (line.Contains("gAbilityNames"))
                    {
                        f = true;
                    }
                    if (line.Contains("};") && f)
                    {
                        break;
                    }
                    if (f)
                    {
                        ablines.Add(line);
                    }
                    line = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot read base stats file");
            }
            f = false;
            foreach(string sk in ablines)
            {
                if (sk.Contains("ABILITY_NONE"))
                {
                    f = true;
                }
                if (f)
                {
                    string s = sk;
                    int i = s.IndexOf('[');
                    int j = s.IndexOf(']');
                    int k = s.IndexOf('\"');
                    int l = s.LastIndexOf('\"');
                    abilityenums.Add(s.Substring(i + 1, j - i - 1));
                    abilitynames.Add(s.Substring(k + 1, l - k - 1));
                }
            }
            //convert all to prevent data aliasing
            comboBox2.DataSource = abilitynames.ConvertAll(x => x);
            comboBox3.DataSource = abilitynames.ConvertAll(x => x);
            comboBox4.DataSource = abilitynames.ConvertAll(x => x);
            comboBox5.DataSource = abilitynames.ConvertAll(x => x);
            comboBox6.DataSource = abilitynames.ConvertAll(x => x);
            comboBox7.DataSource = abilitynames.ConvertAll(x => x);
        }

        //kill me
        public void initMoves()
        {
            string movepath = erpath + "\\src\\data\\text\\move_names.h";
            bool f = false;
            List<String> mvlines = new List<String>();
            moveenums = new List<String>();
            movenames = new List<String>();
            try
            {
                StreamReader sr = new StreamReader(movepath);
                string line = sr.ReadLine();

                while (line != null)
                {
                    if (line.Contains("gMoveNamesLong"))
                    {
                        f = true;
                    }
                    if (line.Contains("};") && f)
                    {
                        break;
                    }
                    if (f)
                    {
                        mvlines.Add(line);
                    }
                    line = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot read move names file");
            }
            f = false;
            foreach (string sk in mvlines)
            {
                if (sk.Contains("MOVE_NONE"))
                {
                    f = true;
                }
                if(!sk.Contains("[") || !sk.Contains("\""))
                {
                    continue;
                }
                if (f)
                {
                    string s = sk;
                    int i = s.IndexOf('[');
                    int j = s.IndexOf(']');
                    int k = s.IndexOf('\"');
                    int l = s.LastIndexOf('\"');
                    moveenums.Add(s.Substring(i + 1, j - i - 1));
                    movenames.Add(s.Substring(k + 1, l - k - 1));
                }
            }
            comboBox9.DataSource = movenames.ConvertAll(x => x);
            movepath = erpath + "\\src\\data\\pokemon\\level_up_learnsets.h";
            mvlines = new List<String>();
            try
            {
                StreamReader sr = new StreamReader(movepath);
                string line = sr.ReadLine();
                string currentPokemon = "";
                levelupmoves = new Dictionary<String, List<String>>();
                bool skip = false;
                while (line != null)
                {
                    if (line.Contains("/*"))
                    {
                        skip = true;
                    }
                    if (line.Contains("*/"))
                    {
                        skip = false;
                    }
                    if (skip)
                    {
                        line = sr.ReadLine();
                        continue;
                    }
                    if(line.Contains("static const struct"))
                    {
                        string[] st = line.Split(' ');
                        foreach (string sk in st)
                        {
                            if (sk.EndsWith("[]"))
                            {
                                int i = sk.IndexOf("LevelUp");
                                currentPokemon = sk.Substring(1, i - 1);
                                levelupmoves[currentPokemon] = new List<String>();
                            }
                        }
                    }
                    if (line.Contains("LEVEL_UP_MOVE") && !line.Contains(".level") && !line.Trim().StartsWith("//"))
                    {
                        try
                        {
                            int i = line.IndexOf('(');
                            int j = line.IndexOf(')');
                            List<String> temp = levelupmoves[currentPokemon];
                            temp.Add(line.Substring(i + 1, j - i - 1));
                            levelupmoves[currentPokemon] = temp;
                            //Console.WriteLine(s.Substring(i + 1, j - i - 1));
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(line);
                            Console.WriteLine(e.ToString());
                        }


                    }



                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            





        }

        //revert to default button
        private void button11_Click(object sender, EventArgs e)
        {
            loadOriginalAbilities();
        }

        //levelup learnset moves button
        private void button5_Click(object sender, EventArgs e)
        {
            if (erpath == null || (comboBox1.SelectedIndex == -1))
            {
                return;
            }
            refreshLevelup();
        }
        public void refreshLevelup()
        {
            levelup = true;
            string currentPokemon = (string)comboBox1.SelectedItem;
            currentPokemon = searchname(currentPokemon);
            comboBox9.DataSource = movenames.ConvertAll(x => x);
            //comboBox8.DataSource = levelupmoves[currentPokemon].ConvertAll(x => x);
            try
            {
                List<String> prettyout = levelupmoves[currentPokemon].ConvertAll(x =>
                {
                    string[] t = x.Split(',');
                    string level = t[0];
                    string move = t[1].Trim();
                    return level + " " + movenames[moveenums.IndexOf(move)];
                }
                            );
                comboBox8.DataSource = prettyout;
            }
            catch
            {
                debugcon.AppendText("Error opening " + currentPokemon + "'s moves. Most likely a form without moves or with moves that don't differ from the original\n.");
            }
            
        }

        //remove move button
        private void button4_Click(object sender, EventArgs e)
        {
            if (erpath == null || (comboBox1.SelectedIndex == -1) || comboBox8.SelectedIndex == - 1)
            {
                return;
            }
            if (levelup)
            {
                string currentPokemon = (string)comboBox1.SelectedItem;
                List<String> temp = levelupmoves[currentPokemon];
                temp.RemoveAt(comboBox8.SelectedIndex);
                levelupmoves[currentPokemon] = temp;
                refreshLevelup();
            }
            
        }
        //add move button
        private void button3_Click(object sender, EventArgs e)
        {
            if (erpath == null || (comboBox1.SelectedIndex == -1) || comboBox9.SelectedIndex == -1)
            {
                return;
            }
            if(levelup)
            {
                int lvl = 0;
                try
                {
                    lvl = int.Parse(textBox1.Text);
                }
                catch
                {
                    return;
                }
                string currentPokemon = (string)comboBox1.SelectedItem;
                List<String> temp = levelupmoves[currentPokemon];
                temp.Add(lvl + ", " + moveenums[comboBox9.SelectedIndex]);
                levelupmoves[currentPokemon] = temp;
                refreshLevelup();
            }




        }

        //apply move changes
        //i want to die
        private void button10_Click(object sender, EventArgs e)
        {
            if (levelup)
            {
                string movepath = erpath + "\\src\\data\\pokemon\\level_up_learnsets.h";
                List<String> mvlines = new List<String>();
                StreamReader sr = new StreamReader(movepath);
                string line = sr.ReadLine();
                while (line != null)
                {
                    mvlines.Add(line);
                    line = sr.ReadLine();
                }
                string currentPokemon = stripname((string)comboBox1.SelectedItem);
                int i = 0;
                while (!mvlines[i].Contains(currentPokemon))
                {
                    i++;
                }
                int start = i;
                //int bracket = start + 1;
                while (!mvlines[i].Contains("};"))
                {
                    i++;
                }
                int endbracket = i;
                while(start != endbracket - 1)
                {
                    mvlines.RemoveAt(start + 1);
                    endbracket--;
                }
                sr.Close();
                List<String> mvs = levelupmoves[currentPokemon];
                mvs.Sort((String x, String y) =>
                {
                    string[] xt = x.Split(',');
                    string[] yt = y.Split(',');

                    return int.Parse(yt[0]) - int.Parse(xt[0]);
                }
                );
                foreach(String sk in mvs)
                {
                    mvlines.Insert(start + 1, "\tLEVEL_UP_MOVE(" + sk + "),");
                }
                using (StreamWriter sw = new StreamWriter(movepath))
                {
                    foreach (String sk in mvlines)
                    {
                        sw.WriteLine(sk);
                    }
                }
            }
        }

        private String stripname(String s)
        {
            string[] st = s.Split('_');
            for(int i = 0; i < st.Length; i++)
            {
                string name = st[i];
                name = char.ToUpper(name[0]) + name.Substring(1);
                st[i] = name;
            }
            return String.Join("",st);
        }
        private String searchname(String s)
        {
            string[] st = s.Split('_');
            for (int i = 0; i < st.Length; i++)
            {
                string name = st[i];
                name = char.ToUpper(name[0]) + name.Substring(1);
                st[i] = name;
            }
            return String.Join("_", st);
        }


        //apply ability changes
        private void button9_Click(object sender, EventArgs e)
        {

            string bspath = erpath + "\\src\\data\\pokemon\\base_stats.h";
            List<String> basestatlines = new List<String>();
            try
            {
                StreamReader sr = new StreamReader(bspath);
                string line = sr.ReadLine();
                while (line != null)
                {
                    basestatlines.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch
            {
                return;
            }
            string currentPokemon = (string) comboBox1.SelectedItem;
            int i = 0;
            while(!basestatlines[i].Contains(currentPokemon.ToUpper()))
            {
                i++;
            }
            while (!basestatlines[i].Contains(".abilities"))
            {
                i++;
            }
            basestatlines[i] = "\t.abilities = {" + abilityenums[comboBox2.SelectedIndex] + ", " + abilityenums[comboBox3.SelectedIndex] + ", " + abilityenums[comboBox4.SelectedIndex] + "},";
            basestatlines[i+1] = "\t.innates = {" + abilityenums[comboBox5.SelectedIndex] + ", " + abilityenums[comboBox6.SelectedIndex] + ", " + abilityenums[comboBox7.SelectedIndex] + "},";
            using (StreamWriter sw = new StreamWriter(bspath))
            {
                foreach (String sk in basestatlines)
                {
                    sw.WriteLine(sk);
                }
            }


        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ATKBAR.Value = int.Parse(atkbox.Text.Trim());
                updateBST();
            }
            catch
            {

            }
        }
        
        
        private void hpbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                HPBAR.Value = int.Parse(hpbox.Text.Trim());
                updateBST();
            }
            catch
            {

            }
            
        }

        private void defbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DEFBAR.Value = int.Parse(defbox.Text.Trim());
                updateBST();
            }
            catch
            {

            }
        }

        private void spabox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SPABAR.Value = int.Parse(spabox.Text.Trim());
                updateBST();
            }
            catch
            {

            }
        }

        private void spdbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SPDBAR.Value = int.Parse(spdbox.Text.Trim());
                updateBST();
            }
            catch
            {

            }
        }

        private void spebox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SPEBAR.Value = int.Parse(spebox.Text.Trim());
                updateBST();
            }
            catch
            {

            }
        }

        private void updateBST()
        {
            int BST = int.Parse(spebox.Text.Trim()) + int.Parse(spdbox.Text.Trim()) + int.Parse(spabox.Text.Trim()) + int.Parse(defbox.Text.Trim()) + int.Parse(atkbox.Text.Trim()) + int.Parse(hpbox.Text.Trim());
            label7.Text = "BST = " + BST;

        }
    }
}
