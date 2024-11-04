Resolver el siguiente problema con PMA. En un negocio de cobros digitales hay P personas que
deben pasar por la única caja de cobros para realizar el pago de sus boletas. Las personas son
atendidas de acuerdo con el orden de llegada, teniendo prioridad aquellos que deben pagar menos
de 5 boletas de los que pagan más. Adicionalmente, las personas embarazadas tienen prioridad sobre
los dos casos anteriores. Las personas entregan sus boletas al cajero y el dinero de pago; el cajero les
devuelve el vuelto y los recibos de pago.

chan filaPrioritaria1(int, texto, double);
chan filaPrioritaria2(int, texto, double);
chan filaComun(int, texto, double);
chan atenderse();
chan resultado[P](double, texto);

process persona [id:0..P]{
    if(esta embarazada)
        send filaPrioritaria1(id, boletas, dinero);
    else if (cantBoletas < 5)
        send filaPrioritaria2(id, boletas, dinero);
    else
        send filaComun(id, boletas, dinero);
    endif;

    // espera a ser atendida

    send atenderse();
    receive resultado[id](vuelto, recibos);
}

process caja {
    for i in 1 to P {
        receive atenderse();

        if(not empty (filaPrioritaria1))
            receive filaPrioritaria1(idP, boletas, dinero);
        else if (not empty (filaPrioritaria2))
            receive filaPrioritaria2(idP, boletas, dinero);
        else
            receive filaComun(idP, boletas, dinero);
        endif;

        vuelto, recibos = procesarPago(boletas, dinero);
        send resultado[idP](vuelto, recibos);
    }
}
