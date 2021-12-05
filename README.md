# FİŞ TARAMA SİSTEMİ
Not: Proje C# dilinde Visual Studio 2022 kullanarak .NET 6.0 ile yapılmıştır.

# AÇIKLAMA VE ÇÖZÜM

Bir fiş tarama sistemi geliştirilecektir. OCR aşaması için SaaS bir sistem kullanılmaktadır.

Sistem Tanımı: Her fiş görseli için SaaS hizmetinden bir adet json response dönülmektedir. Json 
response içerisinde description ve ilgili description için koordinat bilgileri yer almaktadır. Amaç 
json’ın uygun şekilde parse edilerek fişe ait text’in kelimelerinin aynı satırda olanlarını bulup, satır ve satır içinde kelime sırasına göre bir çıktı elde etmek.

Çözüm:
* Her kelimenin 4 köşesinin x ve y koordinatlarını json dosyası içinden aldım.
* Kelimelerin y ekseninde dip ve tepe noktalarını kullanarak kelimenin boyunu elde ettim.
* Kelimelerin boylarının ortalamasını aldım.
* Bulduğum ortalama değeri kullanarak bir treshold değeri belirledim.
* Sırasıyla kelimelerin sol-üst y noktasını referans alarak eşik değeri arasındaki kelimeleri elde ettim
* Bulduğum kelimerleri satır sırasına ve kelime sırasına göre düzenledim
* Sonucu result.txt'ye yazdırdım

# DERLEME VE ÇALIŞTIRMA

Proje, Console Application tipinde oluşturulmuştur. Visual Studio ile doğrudan çalıştırabilirsiniz.

# SONUÇ

Json uygun şekilde parse edilerek koordinatları verilen kelimelerin aynı satırda olanları bulundu. Satırlar ve satır içindeki kelimeler sıralanarak result.txt dosyasına yazdırıldı.
