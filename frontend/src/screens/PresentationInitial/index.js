import React from 'react';
import Gradient from '../../components/Gradient';
import { ImageFIsic, Safezinha, Subtitulo } from './style';
import { Logo } from '../../components/Logo/index';
import { Title } from '../../components/Title/style';
import { Button } from '../../components/Button';
import { ContainerPresentation } from '../Presentation/style';
import { StatusBar, View } from 'react-native';

const Fisic = require('../../assets/Images/background.png');

export const PresentationInitial = ({ navigation }) => {
  return (
    <Gradient>
      <StatusBar translucent barStyle="light-content" />

      <ContainerPresentation>
        <ImageFIsic source={Fisic} />

        <Logo marginTop={"-44"} heightLogo={40} widthLogo={80} />

        <Title FontSize={20} marginTop={"3%"}>Bem Vindo ao{"\n"} Gym Tracker</Title>
        <Subtitulo FontSize={12} marginTop={"2%"}>Seu app de treinos diarios</Subtitulo>

        <Button
          marginTop={"8%"}
          title="Continuar"
          handleClickFn={() => navigation.navigate("LoginScreen")}
        />
      </ContainerPresentation>

    </Gradient>
  );
};

export default PresentationInitial;