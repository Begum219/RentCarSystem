using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.BackgroundJobs
{
    public interface IBackgroundJobService
    {
        void EnqueuePasswordResetEmail(string email, string token);
        void EnqueueVerificationEmail(string email, string token);
        void EnqueueWelcomeEmail(string email, string userName);
    }
}
