using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace BindingExample
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _nombre;
        private string _telefono;
        private bool _puedeGuardar;

        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
                VerificarHabilitacion();
            }
        }

        public string Telefono
        {
            get => _telefono;
            set
            {
                _telefono = value;
                OnPropertyChanged(nameof(Telefono));
                VerificarHabilitacion();
            }
        }

        public bool PuedeGuardar
        {
            get => _puedeGuardar;
            set
            {
                _puedeGuardar = value;
                OnPropertyChanged(nameof(PuedeGuardar));
            }
        }

        public ICommand GuardarCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            GuardarCommand = new RelayCommand(Guardar, () => PuedeGuardar);
        }

        private void VerificarHabilitacion()
        {
            PuedeGuardar = !string.IsNullOrWhiteSpace(Nombre) && !string.IsNullOrWhiteSpace(Telefono);
        }

        private void Guardar()
        {
            try
            {
                string rutaArchivo = "datos.xml";
                XDocument xmlDoc;

                if (File.Exists(rutaArchivo))
                {
                    xmlDoc = XDocument.Load(rutaArchivo);
                    xmlDoc.Root.Add(new XElement("Contacto",
                        new XElement("Nombre", Nombre),
                        new XElement("Telefono", Telefono)));
                }
                else
                {
                    xmlDoc = new XDocument(
                        new XElement("Contactos",
                            new XElement("Contacto",
                                new XElement("Nombre", Nombre),
                                new XElement("Telefono", Telefono)))
                    );
                }

                xmlDoc.Save(rutaArchivo);
                MessageBox.Show("Datos guardados en datos.xml", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                Nombre = string.Empty;
                Telefono = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public void Execute(object parameter) => _execute();
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}