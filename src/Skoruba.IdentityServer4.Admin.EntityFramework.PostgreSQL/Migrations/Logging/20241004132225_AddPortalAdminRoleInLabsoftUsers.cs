using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.Migrations.Logging
{
    public partial class AddPortalAdminRoleInLabsoftUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM public.""Roles"" WHERE ""NormalizedName"" = 'PORTALSYSTEMADMINISTRATOR') THEN
                
                        PERFORM 'SELECT ""Id"" FROM public.""Roles"" WHERE ""NormalizedName"" = ''PORTALSYSTEMADMINISTRATOR'' LIMIT 1';
                
                        INSERT INTO public.""UserRoles"" (""UserId"", ""RoleId"")
                        SELECT ""Id"", (SELECT ""Id"" FROM public.""Roles"" WHERE ""NormalizedName"" = 'PORTALSYSTEMADMINISTRATOR' LIMIT 1)
                        FROM public.""Users""
                        WHERE ""NormalizedEmail"" IN (
                            'ADRIANA.MEDEIROS@LABSOFT.COM.BR',
                            'ADRIANA.OLIVEIRA@LABSOFT.COM.BR',
                            'AIRTON.FABRE@LABSOFT.COM.BR',
                            'ALEJANDRA.ROJAS@LABSOFT.COM.BR',
                            'ALEX@LABSOFT.COM.BR',
                            'ANA.SOUSA@LABSOFT.COM.BR',
                            'ANDERSON.JARDIM@LABSOFT.COM.BR',
                            'ANDREY.CUNHA@LABSOFT.COM.BR',
                            'ANTONIO.JUNIOR@LABSOFT.COM.BR',
                            'ARTUR@LABSOFT.COM.BR',
                            'BEATRIZ.ANGELICO@LABSOFT.COM.BR',
                            'BRENDA.SILVA@LABSOFT.COM.BR',
                            'BRUNO.CARVALHO@LABSOFT.COM.BR',
                            'CAMILA.PRETEROTTO@LABSOFT.COM.BR',
                            'CAROLINA.ARANGUIZ@LABSOFT.COM.BR',
                            'CAROLINA.SPINA@LABSOFT.COM.BR',
                            'CAROLINE.VICENTE@LABSOFT.COM.BR',
                            'CLEBERSON.BERTOLANI@LABSOFT.COM.BR',
                            'CRISTINA.PEREIRA@LABSOFT.COM.BR',
                            'DANIEL.PEDRO@LABSOFT.COM.BR',
                            'DIEGO.BERGANTON@LABSOFT.COM.BR',
                            'DIEGO.MATTION@LABSOFT.COM.BR',
                            'DIEGO.MARCELINO@LABSOFT.COM.BR',
                            'ELIANA.CAVALCANTI@LABSOFT.COM.BR',
                            'ELIZABETH.SILVA@LABSOFT.COM.BR',
                            'ERICK.GODOY@LABSOFT.COM.BR',
                            'EWERTON.NASCIMENTO@LABSOFT.COM.BR',
                            'FERNANDA.PIZOL@LABSOFT.COM.BR',
                            'GABRIEL.SCHROEDER@LABSOFT.COM.BR',
                            'GABRIEL.MELLO@LABSOFT.COM.BR',
                            'GABRIELLE.GARCIA@LABSOFT.COM.BR',
                            'GIOVANNI.MARCHEZINI@LABSOFT.COM.BR',
                            'GLADSTONE.FREITAS@LABSOFT.COM.BR',
                            'GUILHERME.SALVADOR@LABSOFT.COM.BR',
                            'GUSTAVO.RAIMUNDO@LABSOFT.COM.BR',
                            'HEITOR.MELEGATE@LABSOFT.COM.BR',
                            'ISAC.BUENO@LABSOFT.COM.BR',
                            'JANAINA.KRAUSE@LABSOFT.COM.BR',
                            'JESSICA.COSTA@LABSOFT.COM.BR',
                            'JESSICA.ACUIO@LABSOFT.COM.BR',
                            'JOAO.OLIVEIRA@LABSOFT.COM.BR',
                            'JOSE.PAULINO@LABSOFT.COM.BR',
                            'JOAO.CORREA@LABSOFT.COM.BR',
                            'JOAO.PAULO@LABSOFT.COM.BR',
                            'JOAO.MENDES@LABSOFT.COM.BR',
                            'JOAO.PELISSON@LABSOFT.COM.BR',
                            'JOAO.SANTOS@LABSOFT.COM.BR',
                            'JULIANA.GONZALEZ@LABSOFT.COM.BR',
                            'JULIANA.CORREIA@LABSOFT.COM.BR',
                            'JULIANA.NOVENTA@LABSOFT.COM.BR',
                            'LAIS.SUDA@LABSOFT.COM.BR',
                            'LARISSA.ANDRIGHETTI@LABSOFT.COM.BR',
                            'LAIS.BARBOZA@LABSOFT.COM.BR',
                            'LEANDRO.TOFOLI@LABSOFT.COM.BR',
                            'LEANDRO.SCHROEDER@LABSOFT.COM.BR',
                            'LEONARDO.SANTOS@LABSOFT.COM.BR',
                            'LEONARDO.VALENTE@LABSOFT.COM.BR',
                            'LETICIA.STARLING@LABSOFT.COM.BR',
                            'LILIANA.SANTOS@LABSOFT.COM.BR',
                            'LUANA.NISHIMURA@LABSOFT.COM.BR',
                            'LUANDERSON.MOREIRA@LABSOFT.COM.BR',
                            'LUCAS.PINTO@LABSOFT.COM.BR',
                            'LUSIENE.ANASTACIO@LABSOFT.COM.BR',
                            'MAIRA.BAGGIO@LABSOFT.COM.BR',
                            'MARCELO.JUNIOR@LABSOFT.COM.BR',
                            'MARCELO.GRANDE@LABSOFT.COM.BR',
                            'MARCOS.JUNIOR@LABSOFT.COM.BR',
                            'MARIA.LALINDE@LABSOFT.COM.BR',
                            'MARIA.MATIAS@LABSOFT.COM.BR',
                            'MARIANA.CARRARA@LABSOFT.COM.BR',
                            'MARIANA.DANTAS@LABSOFT.COM.BR',
                            'MARIANE.SIQUEIRA@LABSOFT.COM.BR',
                            'MARIO.JUNIOR@LABSOFT.COM.BR',
                            'MATEUS.SILVA@LABSOFT.COM.BR',
                            'MAURICIO.ROSA@LABSOFT.COM.BR',
                            'MAYSA.MAZOLLI@LABSOFT.COM.BR',
                            'MIGUEL.OLIVEIRA@LABSOFT.COM.BR',
                            'MONIQUE.CHINELATTO@LABSOFT.COM.BR',
                            'NATANAEL.SILVA@LABSOFT.COM.BR',
                            'PATRICIA.GIRARDI@LABSOFT.COM.BR',
                            'PAULO.RAMOS@LABSOFT.COM.BR',
                            'PAULO.SIQUEIRA@LABSOFT.COM.BR',
                            'RAPHAEL.PIMENTA@LABSOFT.COM.BR',
                            'RENAN.MACHADO@LABSOFT.COM.BR',
                            'RICARDO.ASSIS@LABSOFT.COM.BR',
                            'ROBERTO.GONCALVES@LABSOFT.COM.BR',
                            'ROBSON.ALTHMAN@LABSOFT.COM.BR',
                            'THAIS.REBERTE@LABSOFT.COM.BR',
                            'THIAGO.LACE@LABSOFT.COM.BR',
                            'THIAGO.ISHIDA@LABSOFT.COM.BR',
                            'THIAGO.MOURAD@LABSOFT.COM.BR',
                            'VAGNER.JOSE@LABSOFT.COM.BR',
                            'VINICIUS.RODEL@LABSOFT.COM.BR',
                            'VINICIUS.TANGI@LABSOFT.COM.BR',
                            'VINICIUS.SANTANA@LABSOFT.COM.BR',
                            'VINICIUS.PEREIRA@LABSOFT.COM.BR',
                            'VITOR.DUARTE@LABSOFT.COM.BR',
                            'VITOR.ROCHA@LABSOFT.COM.BR',
                            'WILSON.TAMASHIRO@LABSOFT.COM.BR'
                        )
                        ON CONFLICT DO NOTHING; 
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    DO $$
                    BEGIN
                        IF EXISTS (SELECT 1 FROM public.""Roles"" WHERE ""NormalizedName"" = 'PORTALSYSTEMADMINISTRATOR') THEN
                
                            DELETE FROM public.""UserRoles""
                            WHERE ""RoleId"" = (SELECT ""Id"" FROM public.""Roles"" WHERE ""NormalizedName"" = 'PORTALSYSTEMADMINISTRATOR' LIMIT 1)
                            AND ""UserId"" IN (
                                SELECT ""Id"" FROM public.""Users"" 
                                WHERE ""NormalizedEmail"" IN (
                                    'ADRIANA.MEDEIROS@LABSOFT.COM.BR',
                                    'ADRIANA.OLIVEIRA@LABSOFT.COM.BR',
                                    'AIRTON.FABRE@LABSOFT.COM.BR',
                                    'ALEJANDRA.ROJAS@LABSOFT.COM.BR',
                                    'ALEX@LABSOFT.COM.BR',
                                    'ANA.SOUSA@LABSOFT.COM.BR',
                                    'ANDERSON.JARDIM@LABSOFT.COM.BR',
                                    'ANDREY.CUNHA@LABSOFT.COM.BR',
                                    'ANTONIO.JUNIOR@LABSOFT.COM.BR',
                                    'ARTUR@LABSOFT.COM.BR',
                                    'BEATRIZ.ANGELICO@LABSOFT.COM.BR',
                                    'BRENDA.SILVA@LABSOFT.COM.BR',
                                    'BRUNO.CARVALHO@LABSOFT.COM.BR',
                                    'CAMILA.PRETEROTTO@LABSOFT.COM.BR',
                                    'CAROLINA.ARANGUIZ@LABSOFT.COM.BR',
                                    'CAROLINA.SPINA@LABSOFT.COM.BR',
                                    'CAROLINE.VICENTE@LABSOFT.COM.BR',
                                    'CLEBERSON.BERTOLANI@LABSOFT.COM.BR',
                                    'CRISTINA.PEREIRA@LABSOFT.COM.BR',
                                    'DANIEL.PEDRO@LABSOFT.COM.BR',
                                    'DIEGO.BERGANTON@LABSOFT.COM.BR',
                                    'DIEGO.MATTION@LABSOFT.COM.BR',
                                    'DIEGO.MARCELINO@LABSOFT.COM.BR',
                                    'ELIANA.CAVALCANTI@LABSOFT.COM.BR',
                                    'ELIZABETH.SILVA@LABSOFT.COM.BR',
                                    'ERICK.GODOY@LABSOFT.COM.BR',
                                    'EWERTON.NASCIMENTO@LABSOFT.COM.BR',
                                    'FERNANDA.PIZOL@LABSOFT.COM.BR',
                                    'GABRIEL.SCHROEDER@LABSOFT.COM.BR',
                                    'GABRIEL.MELLO@LABSOFT.COM.BR',
                                    'GABRIELLE.GARCIA@LABSOFT.COM.BR',
                                    'GIOVANNI.MARCHEZINI@LABSOFT.COM.BR',
                                    'GLADSTONE.FREITAS@LABSOFT.COM.BR',
                                    'GUILHERME.SALVADOR@LABSOFT.COM.BR',
                                    'GUSTAVO.RAIMUNDO@LABSOFT.COM.BR',
                                    'HEITOR.MELEGATE@LABSOFT.COM.BR',
                                    'ISAC.BUENO@LABSOFT.COM.BR',
                                    'JANAINA.KRAUSE@LABSOFT.COM.BR',
                                    'JESSICA.COSTA@LABSOFT.COM.BR',
                                    'JESSICA.ACUIO@LABSOFT.COM.BR',
                                    'JOAO.OLIVEIRA@LABSOFT.COM.BR',
                                    'JOSE.PAULINO@LABSOFT.COM.BR',
                                    'JOAO.CORREA@LABSOFT.COM.BR',
                                    'JOAO.PAULO@LABSOFT.COM.BR',
                                    'JOAO.MENDES@LABSOFT.COM.BR',
                                    'JOAO.PELISSON@LABSOFT.COM.BR',
                                    'JOAO.SANTOS@LABSOFT.COM.BR',
                                    'JULIANA.GONZALEZ@LABSOFT.COM.BR',
                                    'JULIANA.CORREIA@LABSOFT.COM.BR',
                                    'JULIANA.NOVENTA@LABSOFT.COM.BR',
                                    'LAIS.SUDA@LABSOFT.COM.BR',
                                    'LARISSA.ANDRIGHETTI@LABSOFT.COM.BR',
                                    'LAIS.BARBOZA@LABSOFT.COM.BR',
                                    'LEANDRO.TOFOLI@LABSOFT.COM.BR',
                                    'LEANDRO.SCHROEDER@LABSOFT.COM.BR',
                                    'LEONARDO.SANTOS@LABSOFT.COM.BR',
                                    'LEONARDO.VALENTE@LABSOFT.COM.BR',
                                    'LETICIA.STARLING@LABSOFT.COM.BR',
                                    'LILIANA.SANTOS@LABSOFT.COM.BR',
                                    'LUANA.NISHIMURA@LABSOFT.COM.BR',
                                    'LUANDERSON.MOREIRA@LABSOFT.COM.BR',
                                    'LUCAS.PINTO@LABSOFT.COM.BR',
                                    'LUSIENE.ANASTACIO@LABSOFT.COM.BR',
                                    'MAIRA.BAGGIO@LABSOFT.COM.BR',
                                    'MARCELO.JUNIOR@LABSOFT.COM.BR',
                                    'MARCELO.GRANDE@LABSOFT.COM.BR',
                                    'MARCOS.JUNIOR@LABSOFT.COM.BR',
                                    'MARIA.LALINDE@LABSOFT.COM.BR',
                                    'MARIA.MATIAS@LABSOFT.COM.BR',
                                    'MARIANA.CARRARA@LABSOFT.COM.BR',
                                    'MARIANA.DANTAS@LABSOFT.COM.BR',
                                    'MARIANE.SIQUEIRA@LABSOFT.COM.BR',
                                    'MARIO.JUNIOR@LABSOFT.COM.BR',
                                    'MATEUS.SILVA@LABSOFT.COM.BR',
                                    'MAURICIO.ROSA@LABSOFT.COM.BR',
                                    'MAYSA.MAZOLLI@LABSOFT.COM.BR',
                                    'MIGUEL.OLIVEIRA@LABSOFT.COM.BR',
                                    'MONIQUE.CHINELATTO@LABSOFT.COM.BR',
                                    'NATANAEL.SILVA@LABSOFT.COM.BR',
                                    'PATRICIA.GIRARDI@LABSOFT.COM.BR',
                                    'PAULO.RAMOS@LABSOFT.COM.BR',
                                    'PAULO.SIQUEIRA@LABSOFT.COM.BR',
                                    'RAPHAEL.PIMENTA@LABSOFT.COM.BR',
                                    'RENAN.MACHADO@LABSOFT.COM.BR',
                                    'RICARDO.ASSIS@LABSOFT.COM.BR',
                                    'ROBERTO.GONCALVES@LABSOFT.COM.BR',
                                    'ROBSON.ALTHMAN@LABSOFT.COM.BR',
                                    'THAIS.REBERTE@LABSOFT.COM.BR',
                                    'THIAGO.LACE@LABSOFT.COM.BR',
                                    'THIAGO.ISHIDA@LABSOFT.COM.BR',
                                    'THIAGO.MOURAD@LABSOFT.COM.BR',
                                    'VAGNER.JOSE@LABSOFT.COM.BR',
                                    'VINICIUS.RODEL@LABSOFT.COM.BR',
                                    'VINICIUS.TANGI@LABSOFT.COM.BR',
                                    'VINICIUS.SANTANA@LABSOFT.COM.BR',
                                    'VINICIUS.PEREIRA@LABSOFT.COM.BR',
                                    'VITOR.DUARTE@LABSOFT.COM.BR',
                                    'VITOR.ROCHA@LABSOFT.COM.BR',
                                    'WILSON.TAMASHIRO@LABSOFT.COM.BR'
                                )
                            );
                
                        END IF;
                    END $$;
                ");
        }
    }
}
