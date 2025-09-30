using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudoBox.UnifiedModule.Application.Users.Contracts.Auth;

public record RefreshRequest(string RefreshToken);
