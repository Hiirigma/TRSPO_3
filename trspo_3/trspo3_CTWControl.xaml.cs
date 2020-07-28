namespace trspo_3
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using System;
    using System.Text;
    using Microsoft.VisualStudio.Shell;
    using System.IO;
    using System.Linq;

    public partial class trspo3_CTWControl : UserControl
    {
        public System.Windows.SizeToContent SizeToContent { get; set; }
        public System.Windows.ResizeMode ResizeMode { get; set; }

        public trspo3_CTWControl()
        {
            this.InitializeComponent();
            // Resize components
            this.SizeToContent = SizeToContent.Width;
            this.SizeToContent = SizeToContent.Height;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]


        private void removeCommentsConst(ref int count_key, ref int count_comments, string prgm)
        {
            char d;
            char c;
            string res = "";
            for (int i = 0; i < prgm.Length; i++)
            {
                c = prgm[i];
                if (c == '/')
                {
                    i++;
                    d = prgm[i];
                    if (d == '*')
                    {
                        count_comments++;
                        i++;
                        c = prgm[i];
                        i++;
                        d = prgm[i];

                        while ((c != '*' || d != '/') && i < prgm.Length - 1 )
                        {
                            c = d;
                            i++;
                            d = prgm[i];
                        }
                    }
                    else if (d == '/')
                    {
                        count_comments++;
                        do
                        {
                            if (d == '\\' && prgm[i] == '\n')
                            {
                                i++;
                            }
                            d = prgm[i];
                            i++;
                            
                        } while (d != '\n' && i < prgm.Length - 1);
                        i--;
                        continue;
                    }
                }
                else if (c == '\'' || c == '"')
                {
                    i++;
                    d = prgm[i];
                    while (i < prgm.Length - 1)
                    {
                        if (d == c && prgm[i-1] != '\\')
                        {
                            break;
                        }

                        i++;
                        if (d == '\n' && prgm[i - 2] != '\\')
                        {
                            i--;
                            break;
                        }
                        d = prgm[i];
                    }
                }
                else
                {
                    res += c;
                }
            }
            GetWords(ref count_key, ref res);
        }
        
        private void GetWords(ref int cnt_key_words, ref String data)
        {
            String[] keys = {
                            "bool","char","int","long", "signed", "unsigned","float",
                            "double", "int8_t", "int16_t", "int32_t", "int64_t","uint8_t",
                            "uint16_t","uint32_t","uint64_t", "char16_t", "char32_t","struct","short",
                            "register","wchar_t", "DWORD", "void", "HANDLE","FILE","alignas","template",
                            "const","typename","typedef","typeid","union","alignof","and","and_eq","asm",
                            "auto","enum","while","for","break","continue","goto","switch","case","default",
                            "if","else","goto","do","return","not", "not_eq","or", "or_eq", "xor", "xor_eq",
                            "class", "public","private","protected","explicit","this","using","namespace","virtual",
                            "friend","extern","export","static","volatile","inline", "mutable","new","delete","try",
                            "catch","throw","noexcept","static_assert","true","false","decltype","bitand","bitor","compl",
                            "const_cast","static_cast", "dynamic_cast","reinterpret_cast","constexpr", "nullptr", "operator",
                            "sizeof", "thread_local"
                           };


            foreach (String key in keys)
            {
                String str = data;
                int j = 0;
                for (int i = str.IndexOf(key);  i != -1; i = str.IndexOf(key))
                {
                    j = i + key.Length;
                    if (i - 1 >= 0 && ((str[i - 1] >= 'a' && str[i - 1] <= 'z')   ||
                        (str[i - 1] >= '0' && str[i - 1] <= '9') ||
                        (str[i - 1] == '_') ||
                        (str[i - 1] == '\'') ||
                        (str[i - 1] == '\"')))
                    {
                        str = str.Substring(j);
                        continue;
                    }
 
                    if (j < str.Length &&
                        ((str[j] >= 'a' && str[j] <= 'z') ||
                        (str[j] >= '0' && str[j] <= '9') ||
                        (str[j] == '\'') ||
                        (str[j] == '\"')))
                    {
                        str = str.Substring(j);
                        continue;
                    }

                    cnt_key_words++;
                    str = str.Substring(j);
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            int func_start_pos = 0;
            int comments_count = 0;
            int buf_int = 0;
            int lines_Empty = 0;
            int global_lines_Empty = 0;
            int global_comment_count = 0;
            int global_key_word = 0;
            int cnt_key_words = 0;
            int field_count = 0;
            int name_space_count = 0;
            int template_count = 0;
            int class_count = 0;
            bool fnd_flag = false;
            bool fnd_sc_flag = false;
            String all_code = "";
            String[] SourceTextFile;
            String out_res = "";
            int end_pos = 0;

            EnvDTE.DTE dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));

            if (null == dte.ActiveDocument)
            {
                Info.Text = "Solution doesn't load. Can't get information";
                return;
            }

            SourceTextFile = File.ReadAllLines(System.IO.Path.GetFullPath(dte.ActiveDocument.FullName));
            out_res = "Document name :: " + dte.ActiveDocument.FullName + Environment.NewLine + Environment.NewLine;

            for (int i = 0; i < SourceTextFile.Length; ++i)
            {
                all_code = "";
                for (int j = 0; j < SourceTextFile[i].Length; j++)
                {
                    if (SourceTextFile[i][j] == '{' && cnt <= 0)
                    {
                        fnd_flag = false;
                        fnd_sc_flag = false;
                        for (int k = 0; k < j; k++)
                        {
                            buf_int = k;
                            if (SourceTextFile[i][k] != ' ' && SourceTextFile[i][k] != '\t')
                            {
                                break;
                            }
                        }

                        if (buf_int == j)
                        {
                            func_start_pos = i - 1;
                        }
                        else
                        {
                            func_start_pos = i;
                        }

                        for (int l = 0; fnd_flag == false && l < SourceTextFile[func_start_pos].Length && SourceTextFile[func_start_pos][l] != '{'; l++)
                        {
                            fnd_sc_flag = SourceTextFile[func_start_pos][l] == '(' ? true : fnd_sc_flag;
                            fnd_flag = SourceTextFile[func_start_pos][l] == ')' ? true : fnd_flag;
                        }

                        if (fnd_sc_flag == false && fnd_flag == true)
                        {
                            while (fnd_sc_flag == false && func_start_pos >= 0)
                            {
                                func_start_pos--;
                                for (int l = 0; l < SourceTextFile[func_start_pos].Length && SourceTextFile[func_start_pos][l] != '{'; l++)
                                {
                                    fnd_sc_flag = SourceTextFile[func_start_pos][l] == '(' ? true : fnd_sc_flag;
                                }
                            }
                        }

                        if (SourceTextFile[func_start_pos].StartsWith("namespace"))
                        {
                            name_space_count++;
                            continue;
                        }

                        if (SourceTextFile[func_start_pos].StartsWith("class"))
                        {
                            class_count++;
                            continue;
                        }

                        if (SourceTextFile[func_start_pos].StartsWith("template"))
                        {
                            template_count++;
                            continue;
                        }
                        cnt++;
                    }
                    else if (SourceTextFile[i][j] == '}')
                    {
                        if (cnt == 1)
                        {
                            end_pos = i + 1;
                            lines_Empty = 0;
                            cnt_key_words = 0;
                            comments_count = 0;

                            for (int z = func_start_pos; z < end_pos; z++)
                            {
                                if (SourceTextFile[z] == "" || SourceTextFile[z] == "\t" || SourceTextFile[z] == "\n")
                                {
                                    lines_Empty++;
                                    global_lines_Empty++;
                                }
                                else
                                {
                                    all_code += SourceTextFile[z];
                                    if (all_code[all_code.Length - 1] != '\n')
                                    {
                                        all_code += '\n';
                                    }
                                }
                            }
                            removeCommentsConst(ref cnt_key_words, ref comments_count, all_code);
                            field_count = all_code.Count(f => f == '\n');
                            global_comment_count += comments_count;
                            global_key_word += cnt_key_words;
                            out_res += all_code + Environment.NewLine +
                                "Number of fields :: " + field_count.ToString() + Environment.NewLine +
                                "Number of empty fields :: " + lines_Empty.ToString() + Environment.NewLine +
                                "Number of comments :: " + comments_count.ToString() + Environment.NewLine +
                                "Number of keywords :: " + cnt_key_words.ToString() + Environment.NewLine +
                                Environment.NewLine;

                        }
                        if (cnt > 0)
                        {
                            cnt--;
                        }
                    }
                    else
                    {
                        if (SourceTextFile[i][j] == '{')
                        {
                            cnt++;
                        }
                    }
                }
            }

            out_res += Environment.NewLine +
                "All code information :: " + Environment.NewLine +
                "Number of fields :: " + SourceTextFile.Length.ToString() + Environment.NewLine +
                "Number of empty fields :: " + global_lines_Empty.ToString() + Environment.NewLine +
                "Number of comments :: " + global_comment_count.ToString() + Environment.NewLine +
                "Number of keywords :: " + global_key_word.ToString() + Environment.NewLine +
                "Number of namespaces :: " + name_space_count.ToString() + Environment.NewLine +
                "Number of template :: " + template_count.ToString() + Environment.NewLine +
                "Number of classes :: " + class_count.ToString() + Environment.NewLine;

            Info.Text = out_res;
        }
    }




}