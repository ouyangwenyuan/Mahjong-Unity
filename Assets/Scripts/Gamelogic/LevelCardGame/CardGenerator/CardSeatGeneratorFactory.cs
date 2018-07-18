using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSeatGeneratorFactory //: MonoBehaviour 
{
    static CardSeatGeneratorFactory factory;

    public static CardSeatGeneratorFactory GetFactory() {
        if(factory == null){
            factory = new CardSeatGeneratorFactory();
        }

        return factory;
    }

    public ICardSeatGenerator Create(CARD_SHOW_TYPE type, List<CardSeatMonoHandler> list,List<string> card_type, List<int> jewels, List<int> locks, int chocolate_num)
    {
        ICardSeatGenerator generator = new Test3CardSeatGenerator(); //默认Test3

        switch(type){
            
            case CARD_SHOW_TYPE.TEST3:
                generator = new Test3CardSeatGenerator();
                generator.Generator(list, card_type, jewels, locks, chocolate_num);
                break;
        }

        return generator;
    }
}
