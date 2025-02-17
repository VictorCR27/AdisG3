﻿using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using static AdisG3.CursosEstudiantes;

namespace AdisG3
{
    public partial class EnviarAsignacionWindow : Window
    {
        public AsignacionSemana asignacionSemana;
        public int id_estudiante;
        private int id_cursoSeleccionado;
        public int id_asignacionSemana;
        private int asignacionSeleccionada;
        public string nombreCursoSeleccionado { get; set; }

        public int idProfesorSeleccionado { get; set; }
        public int Id_cursoSeleccionado { get; set; }

        public EnviarAsignacionWindow(AsignacionSemana asignacionSemana, int id_estudiante, int id_cursoSeleccionado, int idProfesorSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();
            this.asignacionSemana = asignacionSemana;
            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.idProfesorSeleccionado = idProfesorSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;


            // Set the DataContext to the current instance of EnviarAsignacionWindow (this)
            DataContext = asignacionSemana;

            //MessageBox.Show($"id_estudiante: {id_estudiante}");
            //MessageBox.Show($"Id_cursoSeleccionado {id_cursoSeleccionado}");
            //MessageBox.Show($"idProfesorSeleccionado {idProfesorSeleccionado}");
        }

        // Property to bind to the DataContext in XAML
        public AsignacionSemana Asignacion
        {
            get { return asignacionSemana; }
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            string tareaTexto = txtTareaTexto.Text;
            string tareaArchivo = txtArchivoSeleccionado.Text;

            if (string.IsNullOrWhiteSpace(tareaTexto) && string.IsNullOrWhiteSpace(tareaArchivo))
            {
                MessageBox.Show("Debe completar al menos uno de los campos: Tarea de Texto o Archivo.");
                return;
            }

            string connString = conn_db.GetConnectionString();

            try
            {
                int id_curso = id_cursoSeleccionado;
                int id_profesor = idProfesorSeleccionado;

                // Obtener el id_asignacionSemana correspondiente de la tabla asignacionesSemanas
                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    string asignacionQuery = "SELECT asignacionesSemanas FROM asignacionesSemanas " +
                             "WHERE id_profesor = @id_profesor AND id_curso = @id_curso AND titulo = @titulo";

                    using (MySqlCommand asignacionCommand = new MySqlCommand(asignacionQuery, connection))
                    {
                        asignacionCommand.Parameters.AddWithValue("@id_profesor", id_profesor);
                        asignacionCommand.Parameters.AddWithValue("@id_curso", id_curso);
                        asignacionCommand.Parameters.AddWithValue("@titulo", asignacionSemana.titulo);

                        id_asignacionSemana = Convert.ToInt32(asignacionCommand.ExecuteScalar());
                    }

                    // Check if the student has already sent this assignment in the same week
                    string checkQuery = "SELECT id FROM TareasEnviadas " +
                                        "WHERE profesor = @id_profesor AND curso = @id_curso " +
                                        "AND estudiante = @estudiante AND id_asignacionSemana = @id_asignacionSemana";

                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@id_profesor", id_profesor);
                        checkCommand.Parameters.AddWithValue("@id_curso", id_curso);
                        checkCommand.Parameters.AddWithValue("@estudiante", id_estudiante);
                        checkCommand.Parameters.AddWithValue("@id_asignacionSemana", id_asignacionSemana);

                        object existingAssignmentId = checkCommand.ExecuteScalar();

                        if (existingAssignmentId != null)
                        {
                            // Update the existing assignment with the new data
                            string updateQuery = "UPDATE TareasEnviadas " +
                                                 "SET tareaTXT = @tareaTXT, tareaArchivo = @tareaArchivo " +
                                                 "WHERE id = @existingAssignmentId";

                            using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@tareaTXT", tareaTexto);
                                updateCommand.Parameters.AddWithValue("@tareaArchivo", tareaArchivo);
                                updateCommand.Parameters.AddWithValue("@existingAssignmentId", existingAssignmentId);

                                updateCommand.ExecuteNonQuery();
                            }

                            MessageBox.Show("Tarea actualizada exitosamente.");
                        }
                        else
                        {
                            // Insert the assignment into the TareasEnviadas table
                            string insertQuery = "INSERT INTO TareasEnviadas (profesor, curso, estudiante, tareaTXT, tareaArchivo, id_asignacionSemana) " +
                                                "VALUES (@id_profesor, @id_curso, @estudiante, @tareaTXT, @tareaArchivo, @id_asignacionSemana)";

                            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@id_profesor", id_profesor);
                                insertCommand.Parameters.AddWithValue("@id_curso", id_curso);
                                insertCommand.Parameters.AddWithValue("@estudiante", id_estudiante);
                                insertCommand.Parameters.AddWithValue("@tareaTXT", tareaTexto);
                                insertCommand.Parameters.AddWithValue("@tareaArchivo", tareaArchivo);
                                insertCommand.Parameters.AddWithValue("@id_asignacionSemana", id_asignacionSemana);

                                insertCommand.ExecuteNonQuery();
                            }

                            MessageBox.Show("Tarea enviada exitosamente.");
                        }
                    }
                    CursosEstudiantes CursosEstudiantes = new CursosEstudiantes(id_estudiante, id_cursoSeleccionado, nombreCursoSeleccionado, idProfesorSeleccionado);
                    CursosEstudiantes.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar la tarea: " + ex.Message);
            }
        }

        private void BtnAdjuntarArchivo_Click(object sender, RoutedEventArgs e)
        {
            // Show a file dialog to select the file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos PDF e Imágenes|*.pdf;*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path and display it (you can also save the file to a temporary location if needed)
                string filePath = openFileDialog.FileName;
                txtArchivoSeleccionado.Text = filePath;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CursosEstudiantes CursosEstudiantes = new CursosEstudiantes(id_estudiante, id_cursoSeleccionado, nombreCursoSeleccionado, idProfesorSeleccionado);
            CursosEstudiantes.Show();
            this.Close();
        }

    }
}
