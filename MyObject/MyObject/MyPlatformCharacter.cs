using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Widgets;

class MyPlatformCharacter : PlatformCharacter
{
    private DoubleMeter pisteLaskuri;
    private ProgressBar pistePalkki;
    private int maxVal = 0;

    public MyPlatformCharacter(double leveys, double korkeus, int maxScore)
        : base(leveys, korkeus)
    {
        maxVal = maxScore;
        TeeLaskuri();
        LisaaAse();
    }

    public void AmmuAseella()
    {
        PhysicsObject ammus = Weapon.Shoot();
        if (ammus != null)
        { }
    }

    public void LisaaPiste()
    {
        pisteLaskuri.Value++;
    }

    private void TeeLaskuri()
    {
        pisteLaskuri = new DoubleMeter(0);
        pisteLaskuri.MaxValue = maxVal;
        pisteLaskuri.UpperLimit += SeuraavaKentta;

        pistePalkki = new ProgressBar(25, 5);
        pistePalkki.BorderColor = Color.Red;
        pistePalkki.Y += 30;
        pistePalkki.BindTo(pisteLaskuri);
        Add(pistePalkki);
    }

    private void SeuraavaKentta()
    {
        pistePalkki.BarColor = Color.Green;
    }

    private void LisaaAse()
    {
        AssaultRifle ase = new AssaultRifle(30, 10);
        ase.ProjectileCollision = AmmusOsui;
        ase.Y -= 5;
        Weapon = ase;
    }

    private void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        ammus.Destroy();

        if(kohde.Tag.ToString() == "tahti")
        {
            LisaaPiste();
            kohde.Destroy();
        }
    }
}

