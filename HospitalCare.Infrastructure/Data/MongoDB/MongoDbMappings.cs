using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using HospitalCare.Domain.Entities;

namespace HospitalCare.Infrastructure.Data.MongoDB;

public static class MongoDbMappings
{
    private static bool _isInitialized = false;
    private static readonly object _lock = new();

    public static void RegisterMappings()
    {
        lock (_lock)
        {
            if (_isInitialized) return;

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
            {
                BsonClassMap.RegisterClassMap<Entity>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(e => e.Id)
                        .SetIdGenerator(GuidGenerator.Instance)
                        .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapProperty(e => e.CreatedAt).SetElementName("createdAt");
                    cm.MapProperty(e => e.UpdatedAt).SetElementName("updatedAt");
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Patient)))
            {
                BsonClassMap.RegisterClassMap<Patient>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(p => p.FirstName).SetElementName("firstName");
                    cm.MapProperty(p => p.LastName).SetElementName("lastName");
                    cm.MapProperty(p => p.DateOfBirth).SetElementName("dateOfBirth");
                    cm.MapProperty(p => p.Email).SetElementName("email");
                    cm.MapProperty(p => p.Phone).SetElementName("phone");
                    cm.MapProperty(p => p.Address).SetElementName("address");
                    cm.MapProperty(p => p.Appointments).SetElementName("appointments");
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Doctor)))
            {
                BsonClassMap.RegisterClassMap<Doctor>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(d => d.FirstName).SetElementName("firstName");
                    cm.MapProperty(d => d.LastName).SetElementName("lastName");
                    cm.MapProperty(d => d.Specialization).SetElementName("specialization");
                    cm.MapProperty(d => d.Email).SetElementName("email");
                    cm.MapProperty(d => d.Phone).SetElementName("phone");
                    cm.MapProperty(d => d.LicenseNumber).SetElementName("licenseNumber");
                    cm.MapProperty(d => d.Appointments).SetElementName("appointments");
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Appointment)))
            {
                BsonClassMap.RegisterClassMap<Appointment>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(a => a.PatientId)
                        .SetElementName("patientId")
                        .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapProperty(a => a.DoctorId)
                        .SetElementName("doctorId")
                        .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.MapProperty(a => a.AppointmentDate).SetElementName("appointmentDate");
                    cm.MapProperty(a => a.Duration).SetElementName("duration");
                    cm.MapProperty(a => a.Reason).SetElementName("reason");
                    cm.MapProperty(a => a.Notes).SetElementName("notes");
                    cm.MapProperty(a => a.Status).SetElementName("status");
                    cm.UnmapProperty(a => a.Patient);
                    cm.UnmapProperty(a => a.Doctor);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                BsonClassMap.RegisterClassMap<User>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(u => u.Email).SetElementName("email");
                    cm.MapProperty(u => u.PasswordHash).SetElementName("passwordHash");
                    cm.MapProperty(u => u.FirstName).SetElementName("firstName");
                    cm.MapProperty(u => u.LastName).SetElementName("lastName");
                    cm.MapProperty(u => u.Role).SetElementName("role");
                    cm.MapProperty(u => u.IsActive).SetElementName("isActive");
                });
            }

            _isInitialized = true;
        }
    }
}
