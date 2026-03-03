namespace HospitalCare.Domain.Entities;

public class Prescription : Entity
{
    public Guid AppointmentId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid PatientId { get; private set; }
    public string Diagnosis { get; private set; }
    public List<MedicineDetail> Medicines { get; private set; }
    public string? Notes { get; private set; }
    public DateTime PrescriptionDate { get; private set; }

    private Prescription() { }

    public Prescription(
        Guid appointmentId,
        Guid doctorId,
        Guid patientId,
        string diagnosis,
        List<MedicineDetail> medicines,
        string? notes = null)
    {
        AppointmentId = appointmentId;
        DoctorId = doctorId;
        PatientId = patientId;
        Diagnosis = diagnosis;
        Medicines = medicines;
        Notes = notes;
        PrescriptionDate = DateTime.UtcNow;
    }

    public void UpdateDiagnosis(string diagnosis)
    {
        Diagnosis = diagnosis;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMedicine(MedicineDetail medicine)
    {
        Medicines.Add(medicine);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveMedicine(Guid medicineId)
    {
        Medicines.RemoveAll(m => m.Id == medicineId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class MedicineDetail
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Dosage { get; private set; }
    public string Amount { get; private set; }
    public string Routine { get; private set; }
    public string? Instructions { get; private set; }
    public int DurationDays { get; private set; }

    private MedicineDetail() { }

    public MedicineDetail(
        string name,
        string dosage,
        string amount,
        string routine,
        string? instructions = null,
        int durationDays = 7)
    {
        Id = Guid.NewGuid();
        Name = name;
        Dosage = dosage;
        Amount = amount;
        Routine = routine;
        Instructions = instructions;
        DurationDays = durationDays;
    }
}
