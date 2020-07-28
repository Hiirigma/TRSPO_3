namespace trspo_3
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    //-----------------My Add-----------------------------------------------------------------
    using System;
    using System.Text;
    using Microsoft.VisualStudio.Shell;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    //-----------------End My Add-------------------------------------------------------------

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


        private string DeleteConst(string data)
        {
            int n = data.Length;
            string res = "";

            // Flags to indicate that single line and multpile line comments
            // have started or not.
            bool m_cmt = false;
            int count = 0;
            // Traverse the given program
            for (int i = 0; i < n; i++)
            {
                if (data[i] == '"')
                {
                    count++;
                    i++;
                    while (data[i] != ';')
                    {
                        //if (data[i] == '\\' && data[i + 1] == '\n')
                        //{
                        //    i++;
                        //    continue;
                        //}
                        if (data[i] == '"')
                        {
                            count++;
                        }
                        if (data[i] == ';' && count % 2 == 0)
                        {
                            break;
                        }
                        if (data[i] == '\n' && data[i-1] != '\\')
                        {
                            break;
                        }
                        i++;
                    }


                }
                if (data[i] == '\'')
                {
                    count++;
                    i++;
                    while (data[i] != ';')
                    {
                        //if (data[i] == '\\' && data[i + 1] == '\n')
                        //{
                        //    i++;
                        //    continue;
                        //}
                        if (data[i] == '\'')
                        {
                            count++;
                        }
                        if (data[i] == ';' && count%2 == 0)
                        {
                            break;
                        }
                        if (data[i] == '\n' && data[i-1] != '\\')
                        {
                            break;
                        }
                        i++;
                    }
                }
                res += data[i];
            }
            return res;
        }

        private string removeComments(ref int count_key, ref int count_comments, string prgm)
        {
            char d;
            char c;
            string res = "";
            for (int i = 0; i < prgm.Length; i++)
            {
                //c 
                c = prgm[i];
                if (c == '/')
                {
                    //d
                    i++;
                    d = prgm[i];
                    if (d == '*')
                    {
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
                    else
                    {
                        //res += c;
                        //res += d;
                    }
                }
                else if (c == '\'' || c == '"')
                {
                    //res += c;
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
                    //res += d;
                }
                else
                {
                    res += c;
                }
            }
            CountWords(ref count_key, ref res);
            return res;
        }
        
        //private string removeComments(ref int count_key,ref int count_comments, string prgm)
        //{

        //    string res = "";

        //    // Flags to indicate that single line and multpile line comments
        //    // have started or not.
        //    bool s_cmt = false;
        //    bool m_cmt = false;
        //    bool o_str = false;
        //    bool d_str = false;
        //    int str_cnt = 0;
        //    int n = prgm.Length;

        //    // Traverse the given program
        //    for (int i = 0; i < n; i++)
        //    {
        //        if (prgm[i] == '"')
        //        {
        //            o_str = true;
        //            str_cnt++;
        //        }
        //        // If single line comment flag is on, then check for end of it
        //        if (m_cmt == true && prgm[i] == '\n')
        //        {
        //            if (o_str == true && prgm[i-1] != '\\' && str_cnt%2 == 0)
        //            {
        //                str_cnt = 0;
        //                o_str = false;
        //            }
        //            continue;
        //        }
        //        else if (s_cmt == true && prgm[i-1] == '\\'  && prgm[i] == '\n')
        //        {
        //            continue;
        //        }
        //        else if (s_cmt == true && prgm[i - 1] == '\\' && prgm[i] != '\n')
        //        {
        //            m_cmt = false;
        //            s_cmt = false;
        //            continue;
        //        }
        //        else if (s_cmt == true && prgm[i] == '\n')
        //        {
        //            s_cmt = false;
        //        }
        //        // If multiple line comment is on, then check for end of it
        //        else if (m_cmt == true && prgm[i] == '*' && prgm[i + 1] == '/')
        //        {
        //            m_cmt = false;
        //            i++;
        //        }
        //        // If this character is in a comment, ignore it
        //        else if (s_cmt || m_cmt)
        //        {
        //            continue;
        //        }

        //        // Check for beginning of comments and set the approproate flags
        //        else
        //        {
        //            if (prgm[i] == '/' && prgm[i + 1] == '/')
        //            {
        //                count_comments++;
        //                s_cmt = true;
        //                i++; 
        //            }
        //            else
        //            {
        //                if (prgm[i] == '/' && prgm[i + 1] == '*')
        //                {
        //                    count_comments++;
        //                    m_cmt = true;
        //                    i++;
        //                }
        //                // If current character is a non-comment character, append it to res
        //                else if (prgm[i] != '\t')
        //                {
        //                   res += prgm[i];      
        //                }
        //            }
        //        }
        //    }

        //    res = DeleteConst(res);
        //    CountWords(ref count_key, ref res);
        //    return res;
        //}

        private bool CheckInStr(String data, int st_pos, bool reverse)
        {
            int len = data.Length;
            if (reverse == false)
            {
                for (int i = st_pos; i < len; i++)
                {
                    if (data[i] == ';' || data[i] == ')' || data[i] == '>' || data[i] == '=')
                    {
                        if (CheckInStr(data,st_pos,true) == false)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    if (data[i] == '"' || data[i] == '\'' || (data[i] == '\\' && data[i + 1] == '\n'))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = st_pos; i >= 0; i--)
                {
                    if (data[i] == ';' || data[i] == '(' || data[i] == '<')
                    {
                            return false;
                    }
                    if (data[i] == '"' || data[i] == '\'' || (data[i] == '\\' && data[i+1] == '\n'))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        
        private void CountWords(ref int amount_key_words, ref String data)
        {
            String[] KeyWords = {
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


            foreach (String key in KeyWords)
            {

                String str = data;
                while (true)
                {

                    int idx = str.IndexOf(key);
                    if (idx == -1)
                    {
                        break;
                    }

                    
                    if (idx - 1 >= 0 && ((str[idx - 1] >= 'a' && str[idx - 1] <= 'z')   ||
                        (str[idx - 1] >= '0' && str[idx - 1] <= '9') ||
                        (str[idx - 1] == '_') ||
                        (str[idx - 1] == '\'') ||
                        (str[idx - 1] == '\"')))
                    {
                        str = str.Substring(idx + key.Length);
                        continue;
                    }

                    int idx_plus = idx + key.Length;
                    if (idx_plus < str.Length &&
                        ((str[idx_plus] >= 'a' && str[idx_plus] <= 'z') ||
                        (str[idx_plus] >= '0' && str[idx_plus] <= '9') ||
                        (str[idx_plus] == '\'') ||
                        (str[idx_plus] == '\"')))
                    {
                        str = str.Substring(idx + key.Length);
                        continue;
                    }

                    //if (CheckInStr(str, idx, false) == false)
                    //{
                    //    amount_key_words++;
                    //}
                    amount_key_words++;
                    str = str.Substring(idx_plus);


                }
            }
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            int line_func_name = -1;
            int save_line_bkt = 0;
            int comments_count = 0;
            int buf_int = 0;
            int lines_Empty = 0;
            int global_lines_Empty = 0;
            int global_field_count = 0;
            int amount_key_words = 0;
            int field_count = 0;
            int name_space_count = 0;
            int template_count = 0;
            int class_count = 0;
            bool symb_found = false;
            bool found = false;
            bool found_scob = false;
            String func_code = "";
            String all_code = "";
            String[] TextCode;
            String result = "";
            int start_pos = 0;
            int end_pos = 0;

            EnvDTE.DTE dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));

            if (null == dte.ActiveDocument)
            {
                Info.Text = "Solution doesn't load. Can't get information";
                return;
            }

            TextCode = File.ReadAllLines(System.IO.Path.GetFullPath(dte.ActiveDocument.FullName));
            result = "Document name :: " + dte.ActiveDocument.FullName + Environment.NewLine + Environment.NewLine;

            for (int i = 0; i < TextCode.Length; ++i)
            {
                all_code = "";
                for (int j = 0; j < TextCode[i].Length; ++j)
                {
                    if (TextCode[i][j] == '{')
                    {
                        if (counter > 0)
                        {
                            counter++;
                            continue;
                        }
                        // searching for for first not empty symbol
                        // if k == j - not found
                        // k < j - found
                        for (int k = 0; k < j; k++)
                        {
                            buf_int = k;
                            if (TextCode[i][k] != ' ' && TextCode[i][k] != '\t')
                            {      
                                break;
                            }
                        }

                        // if k==j then line with func_name is earlier
                        line_func_name = i - (((buf_int == j) == true) ? 1 : 0);

                        // searching for ')' in line_func_name before '{' and '\n'
                        found = false;
                        found_scob = false;
                        for (int l = 0; l < TextCode[line_func_name].Length && TextCode[line_func_name][l] != '{'; ++l)
                        {
                            if (TextCode[line_func_name][l] == '(')
                            {
                                save_line_bkt = line_func_name;
                                found_scob = true;
                            }
                            if (TextCode[line_func_name][l] == ')')
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found_scob && found)
                        {
                            for (int h = 0; h < 10; h++)
                            {
                                line_func_name--;
                                for (int l = 0; l < TextCode[line_func_name].Length && TextCode[line_func_name][l] != '{'; ++l)
                                {
                                    if (TextCode[line_func_name][l] == '(')
                                    {
                                        save_line_bkt = line_func_name;
                                        found_scob = true;
                                    }
                                }
                                if (found_scob)
                                {
                                    break;
                                }
                            }
                        }

                        if (TextCode[line_func_name].StartsWith("namespace"))
                        {
                            name_space_count++;
                            continue;
                        }

                        if (TextCode[line_func_name].StartsWith("class"))
                        {
                            class_count++;
                            continue;
                        }

                        if (TextCode[line_func_name].StartsWith("template"))
                        {
                            template_count++;
                            continue;
                        }

                        if (counter == 0)
                        {
                            start_pos = i;
                            if (TextCode[i].StartsWith("{"))
                            {
                                start_pos--;
                            }
                        }
                        counter++;
                    }
                    else if (TextCode[i][j] == '}')
                    {
                        if (counter == 1)
                        {
                            end_pos = i + 1;
                            lines_Empty = 0;
                            amount_key_words = 0;
                            comments_count = 0;
                            symb_found = false;
                            start_pos = line_func_name;
                            for (j = 0; j < TextCode[line_func_name].Length; j++)
                            {
                                char c;
                                c = TextCode[line_func_name][j];
                                if (c == '{')
                                {
                                    break;
                                }

                                if (c != '\t' && c != ' ' && symb_found == false)
                                {
                                    symb_found = true;
                                }
                            }

                            for (int z = start_pos ; z < end_pos; z++)
                            {
                                if (TextCode[z] == "" || TextCode[z] == "\t" || TextCode[z] == "\n")
                                {
                                    lines_Empty++;
                                    global_lines_Empty++;
                                }
                                else
                                {
                                    all_code += TextCode[z];
                                    if (all_code[all_code.Length - 1] != '\n')
                                    {
                                        all_code += '\n';
                                    }
                                }
                            }
                            
                            //all_code = all_code.Remove(all_code.Length - 1, 1);
                            func_code = removeComments(ref amount_key_words,ref comments_count, all_code);
                            while (func_code[func_code.Length-1] == '\n')
                            {
                                func_code = func_code.Remove(func_code.Length - 1, 1);
                            }
                            
                            field_count = func_code.Count(f => f == '\n') + 1;
                            global_field_count += field_count;
                            result += func_code + Environment.NewLine +
                                "Number of fields :: " + field_count.ToString() + Environment.NewLine +
                                "Number of empty fields :: " + lines_Empty.ToString() + Environment.NewLine +
                                "Number of comments :: " + comments_count.ToString() + Environment.NewLine +
                                "Number of keywords :: " + amount_key_words.ToString() + Environment.NewLine + 
                                Environment.NewLine;          
                        }
                        if (counter > 0)
                        {
                            counter--;
                        }
                    }
                }
            }


            all_code = "";
            lines_Empty = 0;
            for (int z = 0; z < TextCode.Length; z++)
            {

                if (TextCode[z] == "" || TextCode[z] == "\t" || TextCode[z] == "\n")
                {
                    lines_Empty++;
                }
                else
                {
                    all_code += TextCode[z];
                    all_code += '\n';
                }
            }

            all_code = all_code.Remove(all_code.Length - 1, 1);
            func_code = removeComments(ref amount_key_words, ref comments_count, all_code);
            while (func_code[func_code.Length - 1] == '\n')
            {
                func_code = func_code.Remove(func_code.Length - 1, 1);
            }

            result += Environment.NewLine +
                "All code information :: " + Environment.NewLine +
                "Number of fields :: " + global_field_count.ToString() + Environment.NewLine +
                "Number of empty fields :: " + global_lines_Empty.ToString() + Environment.NewLine +
                "Number of comments :: " + comments_count.ToString() + Environment.NewLine +
                "Number of namespaces :: " + name_space_count.ToString() + Environment.NewLine +
                "Number of template :: " + template_count.ToString() + Environment.NewLine +
                "Number of classes :: " + class_count.ToString() + Environment.NewLine;

            Info.Text = result;
        }
    }




}