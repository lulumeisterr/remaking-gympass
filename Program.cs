// Ler o arquivo.

using System.Globalization;

string formatHorarioCorrida = "HH:mm:ss.fff";
DateTime referenceDate = DateTime.MinValue.Date; //Representa a menor data possível no DateTime, mas mantemos apenas a parte de tempo.

List<string> resultFiles = File.ReadAllLines("Files/logCorrida.txt").Skip(1).ToList();
List<Corrida> corrida = new List<Corrida>();
Corrida pilotoVencedor = null;

// Preenche o objeto
var resultFilesLength = resultFiles.Count();
for (int i = 0; i < resultFilesLength; i++)
{
    var items = resultFiles[i].Split(";");
    var corridaItem = new Corrida
    {
        Hora = DateTime.ParseExact(items[0], formatHorarioCorrida, null),
        NmPiloto = items[1],
        QtdVolta = Int32.Parse(items[2]),
        TempoVolta = referenceDate.Add(TimeSpan.ParseExact(items[3], @"h\:mm\.fff", null)),
        TempoMedio = double.Parse(items[4], CultureInfo.GetCultureInfo("pt-BR"))
    };

    // Verifica se o piloto vencedor ainda não foi definido ou se o piloto atual tem um tempo de chegada menor
    if (pilotoVencedor == null || corridaItem.Hora < pilotoVencedor.Hora)
    {
        pilotoVencedor = corridaItem;
    }
    corrida.Add(corridaItem);
    items = null;
}

if (pilotoVencedor != null)
{
    Console.WriteLine($"Piloto Vencedor {pilotoVencedor.NmPiloto}");
}

MelhorVolta(corrida);
VelocidadeMediaDeCadaPiloto(corrida);
TempoCadaPilotoChegouPosVencedor(corrida);

// Descobrir a melhor volta da corrida
static void MelhorVolta(List<Corrida> corrida)
{
    var melhorVolta = corrida.OrderBy(c => c.TempoVolta).First();

    Console.WriteLine("Melhor volta da corrida:");
    Console.WriteLine("----------------------------------");
    Console.WriteLine($"| Piloto: {melhorVolta.NmPiloto} |");
    Console.WriteLine($"| Tempo: {melhorVolta.TempoVolta} |");
    Console.WriteLine("----------------------------------");
}

// Calcular a velocidade média de cada piloto durante toda a corrida
static void VelocidadeMediaDeCadaPiloto(List<Corrida> corrida)
{
    var velocidadeMediaPorPiloto = corrida.GroupBy(c => c.NmPiloto).Select(g => new
    {
        Piloto = g.Key,
        VelocidadeMedia = g.Average(c => c.TempoMedio)
    });

    Console.WriteLine("Velocidade média de cada piloto durante toda a corrida:");
    Console.WriteLine("-------------------------------");
    Console.WriteLine("| Piloto   | Velocidade Média |");
    Console.WriteLine("-------------------------------");

    foreach (var item in velocidadeMediaPorPiloto)
    {
        Console.WriteLine($"Piloto: {item.Piloto}, Velocidade Média: {item.VelocidadeMedia}");
    }
}

// Descobrir quanto tempo cada piloto chegou após o vencedor
static void TempoCadaPilotoChegouPosVencedor(List<Corrida> corrida)
{
    var vencedor = corrida.OrderBy(c => c.Hora).First();
    var tempoChegadaPilotos = corrida
    .Select(c => new
    {
        Piloto = c.NmPiloto,
        TempoChegada = (c.Hora - vencedor.Hora).TotalSeconds
    });

    Console.WriteLine("Tempo que cada piloto chegou após o vencedor:");
    Console.WriteLine("------------------------------------");
    Console.WriteLine("| Piloto   | Tempo de Chegada (s) |");
    Console.WriteLine("------------------------------------");

    foreach (var item in tempoChegadaPilotos)
    {
        Console.WriteLine($"Piloto: {item.Piloto}, Tempo de Chegada após o Vencedor: {item.TempoChegada} segundos");
    }
}