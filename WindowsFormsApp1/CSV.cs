using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class CSV : Form
    {
        string fullpath = @"C:\";

        public CSV()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// CSVファイルを選択します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSelect_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = openFileDialog1;

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "default.csv";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "CSV UTF-8 (コンマ区切り)(*.csv)|*.csv";
            //タイトルを設定する
            ofd.Title = "ファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fullpath = ofd.FileName;

                string path = Path.GetFileName(ofd.FileName);

                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                textBox1.Text = path;
            }
        }

        private void csvbutton(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(fullpath, System.Text.Encoding.UTF8);

            CsvToArrayList2(sr.ReadToEnd());
        }

        /// <summary>
        /// CSVをArrayListに変換
        /// https://dobon.net/vb/dotnet/file/readcsvfile.html#section6
        /// </summary>
        /// <param name="csvText">CSVの内容が入ったString</param>
        /// <returns>変換結果のArrayList</returns>
        public static System.Collections.ArrayList CsvToArrayList2(string csvText)
        {
            //前後の改行を削除しておく
            csvText = csvText.Trim(new char[] { '\r', '\n' });

            System.Collections.ArrayList csvRecords =
                new System.Collections.ArrayList();
            System.Collections.ArrayList csvFields =
                new System.Collections.ArrayList();

            int csvTextLength = csvText.Length;
            int startPos = 0, endPos = 0;
            string field = "";

            while (true)
            {
                //空白を飛ばす
                while (startPos < csvTextLength &&
                    (csvText[startPos] == ' ' || csvText[startPos] == '\t'))
                {
                    startPos++;
                }

                //データの最後の位置を取得
                if (startPos < csvTextLength && csvText[startPos] == '"')
                {
                    //"で囲まれているとき
                    //最後の"を探す
                    endPos = startPos;
                    while (true)
                    {
                        endPos = csvText.IndexOf('"', endPos + 1);
                        if (endPos < 0)
                        {
                            throw new ApplicationException("\"が不正");
                        }
                        //"が2つ続かない時は終了
                        if (endPos + 1 == csvTextLength || csvText[endPos + 1] != '"')
                        {
                            break;
                        }
                        //"が2つ続く
                        endPos++;
                    }

                    //一つのフィールドを取り出す
                    field = csvText.Substring(startPos, endPos - startPos + 1);
                    //""を"にする
                    field = field.Substring(1, field.Length - 2).Replace("\"\"", "\"");

                    endPos++;
                    //空白を飛ばす
                    while (endPos < csvTextLength &&
                        csvText[endPos] != ',' && csvText[endPos] != '\n')
                    {
                        endPos++;
                    }
                }
                else
                {
                    //"で囲まれていない
                    //カンマか改行の位置
                    endPos = startPos;
                    while (endPos < csvTextLength &&
                        csvText[endPos] != ',' && csvText[endPos] != '\n')
                    {
                        endPos++;
                    }

                    //一つのフィールドを取り出す
                    field = csvText.Substring(startPos, endPos - startPos);
                    //後の空白を削除
                    field = field.TrimEnd();
                }

                //フィールドの追加
                csvFields.Add(field);

                //行の終了か調べる
                if (endPos >= csvTextLength || csvText[endPos] == '\n')
                {
                    //行の終了
                    //レコードの追加
                    csvFields.TrimToSize();
                    csvRecords.Add(csvFields);
                    csvFields = new System.Collections.ArrayList(
                        csvFields.Count);

                    if (endPos >= csvTextLength)
                    {
                        //終了
                        break;
                    }
                }

                //次のデータの開始位置
                startPos = endPos + 1;
            }

            csvRecords.TrimToSize();
            return csvRecords;
        }
    }
}
