namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;

    public interface IFake : INotifyPropertyChanged
    {
        int WriteOnly { set; }
      
        bool IsTrue { get; set; }
        
        string Name { get; set; }
        
        Level Next { get; set; }
        
        StructLevel StructLevel { get; set; }
        
        NotInpc NotInpc { get; }
        
        int Value { get; set; }

        Level Method();
    }
}