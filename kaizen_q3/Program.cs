//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Text;

// response.json dosyasını okudum
string path = File.ReadAllText(@"..\..\..\response.json");
dynamic json = JsonConvert.DeserializeObject(path);

// sonuçları tutacak tabloyu oluşturdum
DataTable dt_result = new DataTable();
dt_result.Columns.Add(new DataColumn("text"));

// alttaki datatable column'larını oluşturan başlıkların tanımları
// x_lt: x of left-top
// x_lb: x of left-bottom
// x_rt: x of right-top
// x_rb: x of right-bottom

// y_lt: y of left-top
// y_lb: y of left-bottom
// y_rt: y of right-top
// y_rb: y of right-bottom

// json'dan gelecek veriyi tutacak tablo
DataTable dt_data = new DataTable();
dt_data.Columns.Add(new DataColumn("text"));
dt_data.Columns.Add(new DataColumn("x_lt", typeof(int)));
dt_data.Columns.Add(new DataColumn("x_lb", typeof(int)));
dt_data.Columns.Add(new DataColumn("x_rt", typeof(int)));
dt_data.Columns.Add(new DataColumn("x_rb", typeof(int)));
dt_data.Columns.Add(new DataColumn("y_lt", typeof(int)));
dt_data.Columns.Add(new DataColumn("y_lb", typeof(int)));
dt_data.Columns.Add(new DataColumn("y_rt", typeof(int)));
dt_data.Columns.Add(new DataColumn("y_rb", typeof(int)));

// treshold kullanılarak gruplanmış veriyi tutacak tablo
DataTable dt_data_grouped = new DataTable();
dt_data_grouped.Columns.Add(new DataColumn("text"));
dt_data_grouped.Columns.Add(new DataColumn("x_lt", typeof(int)));
dt_data_grouped.Columns.Add(new DataColumn("x_lb", typeof(int)));
dt_data_grouped.Columns.Add(new DataColumn("x_rt", typeof(int)));
dt_data_grouped.Columns.Add(new DataColumn("x_rb", typeof(int)));
dt_data_grouped.Columns.Add(new DataColumn("refer", typeof(int)));


int i = 0;
int x = 0;
double treshold = 0;
string description = "";
dynamic word = null;
DataRow dr;
double wide_range = 0;
// json'daki veriyi dt_data'ya aktardım
for (i = 1; i < json.Count; i++)
{

    description = json[i].description;
    word = json[i].boundingPoly.vertices;

    dr = dt_data.NewRow();
    dr["text"] = description;
    dr["x_lt"] = word[0].x;
    dr["x_lb"] = word[3].x;
    dr["x_rt"] = word[1].x;
    dr["x_rb"] = word[2].x;
    dr["y_lt"] = word[0].y;
    dr["y_lb"] = word[3].y;
    dr["y_rt"] = word[1].y;
    dr["y_rb"] = word[2].y;

    double low_start = 0;
    double high_end = 0;

    // kelimelerin y ekseninde tepe ve diplerini bularak, max boyunu buldum
    if (word[0].y > word[1].y)
    {
        low_start = word[1].y;
    }
    else
    {
        low_start = word[0].y;
    }

    if (word[2].y > word[3].y)
    {
        high_end = word[2].y;
    }
    else
    {
        high_end = word[3].y;
    }

    wide_range += (high_end - low_start);
    dt_data.Rows.Add(dr);

}
// kelimelerin ortalama boyunu buldum ve 
// y ekseninde aynı satırda olan kelimeleri taramak için tresholdu elde ettim
double avg = Math.Ceiling((wide_range / dt_data.Rows.Count) / 2);
treshold = avg;

// kelimelere sırasıyla bakarak, sırası gelen kelimenin y_lt ekseninin 10+- aralığında başka başka kelimelerin y_lt var mı diye baktım
// bulduklarım aynı hizadadır
// bulduklarımı sildim, sırasıyla kalan kelimelere aynı işlemi uyguladım
// bulduğum kelimelerin referans y_lt değerine arama yaptığım kelimenin y_lt'sini verdim
START:
for (i = 0; i < dt_data.Rows.Count; i++)
{
    int y_lt = dt_data.Rows[i].Field<int>("y_lt");
    DataRow[] ro = dt_data.Select("(y_lt < " + (dt_data.Rows[i].Field<int>("y_lt") + treshold) + ") and (y_lt > " + (dt_data.Rows[i].Field<int>("y_lt") - treshold) + ")");

    foreach (DataRow row in ro)
    {

        dr = dt_data_grouped.NewRow();
        dr["text"] = row.Field<string>("text");
        dr["x_lt"] = row.Field<int>("x_lt");
        dr["x_lb"] = row.Field<int>("x_lb");
        dr["x_rt"] = row.Field<int>("x_rt");
        dr["x_rb"] = row.Field<int>("x_rb");
        dr["refer"] = y_lt;
        dt_data_grouped.Rows.Add(dr);

    }

    for (x = 0; x < ro.Length; x++)
        ro[x].Delete();
    dt_data.AcceptChanges();

    goto START;

}


// satır bazında gruplandırdığım dataya sırasıyla baktım
// bulduklarımı x_lt ye göre(başlangıç x) sıraladım
// sıraldaıklarım satır halinde sonuç tabloma ekledim
string text = "";
int refer = 0;
for (i = 0; i < dt_data_grouped.Rows.Count; i++)
{
    refer = dt_data_grouped.Rows[i].Field<int>("refer");
    DataRow[] ro = dt_data_grouped.Select("refer =" + refer + "", "x_lt ASC");
    text = "";
    foreach (DataRow row in ro)
    {
        if (text == "")
        {
            text = row.Field<string>("text");
        }
        else
        {
            text += " " + row.Field<string>("text");
        }
    }

    dr = dt_result.NewRow();
    dr["text"] = text;
    dt_result.Rows.Add(dr);

    // pass the data and fetch next ones
    i = i + ro.Length - 1;

}

// sonuç tablom satır sırasına ve kelime sırasına göre oluştu
// sb ye satır satır ekleyedim
StringBuilder sb = new StringBuilder();
sb.AppendLine("line | text");
for (i = 0; i < dt_result.Rows.Count; i++)
{
    if ((i + 1) < 10)
    {
        sb.AppendLine((i + 1) + "    | " + dt_result.Rows[i].Field<string>("text"));
    }
    else
    {
        sb.AppendLine((i + 1) + "   | " + dt_result.Rows[i].Field<string>("text"));
    }

}

// sb'yi result.txt'ye yazdırdım
using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\..\result.txt"))
{
    file.WriteLine(sb.ToString());
}


Console.WriteLine("Fiş satır satır result.txt'ye yazdırılmıştır. Çıkış yapmak için Enter'a basınız.");
Console.ReadLine();


